using AppWorkflow.Core.Domain.Data;
using AppWorkflow.Core.Interfaces.Services;
using AppWorkflow.Infrastructure.Data.Context;
using AppWorkflow.Services.Interfaces;
using Microsoft.Extensions.Caching.Distributed;

namespace AppWorkflow.Infrastructure.Services
{
    public class WorkflowStateManager : IWorkflowStateManager
    {
        private readonly WorkflowDbContext _dbContext;
        private readonly IDistributedLockManager _lockManager;
        private readonly ILogger<WorkflowStateManager> _logger;
        private readonly IDistributedCache _cache;
        private readonly IServiceProvider serviceProvider;
        public WorkflowStateManager(
            WorkflowDbContext dbContext,
            IDistributedLockManager lockManager,
            IDistributedCache cache,
            ILogger<WorkflowStateManager> logger,
            IServiceProvider serviceProvider)
        {
            _dbContext = dbContext;
            _lockManager = lockManager;
            _cache = cache;
            _logger = logger;
            this.serviceProvider = serviceProvider;
        }

        public async Task SaveStateAsync(WorkflowExecutionContext context)
        {
            using var lockScope = await _lockManager.AcquireLockAsync(
                $"workflow-state-{context.InstanceId}",
                TimeSpan.FromSeconds(30)
            );

            try
            {
                // Get existing workflow data
                var workflowData = await _dbContext.WorkflowDatas
                    .Include(w => w.StepInstances)
                    .FirstOrDefaultAsync(w => w.Id == context.InstanceId);

                if (workflowData == null)
                {
                    throw new WorkflowNotFoundException(context.WorkflowId, context.InstanceId);
                }

                // Update workflow state
                workflowData.Status = context.Status;
                workflowData.CurrentStepId = context.CurrentStepId;
                workflowData.Variables = context.Variables;
                workflowData.UpdatedAt = DateTime.UtcNow;

                // Update step data
                var currentStep = workflowData.StepInstances
                    .FirstOrDefault(s => s.StepId == context.CurrentStepId);

                if (currentStep != null)
                {
                    currentStep.StepVariables = context.StepData;
                    currentStep.UpdatedAt = DateTime.UtcNow;
                }

                // Save metadata
                foreach (var (key, value) in context.Metadata)
                {
                    workflowData.AuditLog += $"{DateTime.UtcNow:s} - {key}: {value}\n";
                }

                // Save execution path
                foreach (var path in context.ExecutionPath)
                {
                    workflowData.AuditLog += $"{path}\n";
                }

                await _dbContext.SaveChangesAsync();

                // Cache the latest state
                var cacheKey = $"workflow-state-{context.InstanceId}";
                var cacheOptions = new DistributedCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromMinutes(30)
                };

                await _cache.SetAsync(cacheKey,
                    JsonSerializer.SerializeToUtf8Bytes(context),
                    cacheOptions);

                _logger.LogInformation(
                    "Saved workflow state for instance {InstanceId}, Status: {Status}, Step: {StepId}",
                    context.InstanceId,
                    context.Status,
                    context.CurrentStepId
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error saving workflow state for instance {InstanceId}",
                    context.InstanceId);
                throw;
            }
        }

        public async Task<WorkflowExecutionContext> LoadStateAsync(Guid instanceId)
        {
            // Try cache first
            var cacheKey = $"workflow-state-{instanceId}";
            var cachedState = await _cache.GetAsync(cacheKey);

            if (cachedState != null)
            {
                return JsonSerializer.Deserialize<WorkflowExecutionContext>(cachedState);
            }

            // Load from database
            var workflowData = await _dbContext.WorkflowDatas
                .Include(w => w.StepInstances)
                .FirstOrDefaultAsync(w => w.Id == instanceId);

            if (workflowData == null)
            {
                throw new WorkflowNotFoundException(message: $"Workflow instance {instanceId} not found");
            }

            // Create execution context
            var context = new WorkflowExecutionContext(
                workflowData.WorkflowId,
                workflowData.Id,
                serviceProvider)
            {
                Status = workflowData.Status,
                CurrentStepId = workflowData.CurrentStepId,
                Variables = workflowData.Variables,
                ModuleData = JsonSerializer.Deserialize<dynamic>(workflowData.ModuleData),
                StartTime = workflowData.StartedAt
            };

            // Load current step data
            var currentStep = workflowData.StepInstances
                .FirstOrDefault(s => s.StepId == workflowData.CurrentStepId);

            if (currentStep != null)
            {
                context.StepData = currentStep.StepVariables;
            }

            // Parse execution path from audit log
            if (!string.IsNullOrEmpty(workflowData.AuditLog))
            {
                context.ExecutionPath = workflowData.AuditLog
                    .Split('\n')
                    .Where(l => !string.IsNullOrEmpty(l))
                    .ToList();
            }

            return context;
        }

        public async Task<bool> HasActiveCheckpointAsync(Guid instanceId)
        {
            return await _dbContext.WorkflowDatas
                .AnyAsync(w => w.Id == instanceId &&
                             w.StepInstances.Any(s => s.Status == StepStatus.Completed));
        }

        public async Task CreateCheckpointAsync(Guid instanceId)
        {
            using var lockScope = await _lockManager.AcquireLockAsync(
                $"workflow-checkpoint-{instanceId}",
                TimeSpan.FromSeconds(30)
            );

            var workflowData = await _dbContext.WorkflowDatas
                .Include(w => w.StepInstances)
                .FirstOrDefaultAsync(w => w.Id == instanceId);

            if (workflowData == null)
            {
                throw new WorkflowNotFoundException(message: $"Workflow instance {instanceId} not found");
            }

            // Create checkpoint snapshot
            var checkpoint = new WorkflowCheckpoint
            {
                InstanceId = instanceId,
                Status = workflowData.Status,
                Variables = workflowData.Variables,
                CurrentStepId = workflowData.CurrentStepId,
                CheckpointTime = DateTime.UtcNow,
                StepData = workflowData.StepInstances
                    .ToDictionary(s => s.StepId, s => s.StepVariables)
            };

            await _dbContext.WorkflowCheckpoints.AddAsync(checkpoint);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation(
                "Created checkpoint for workflow instance {InstanceId} at step {StepId}",
                instanceId,
                workflowData.CurrentStepId);
        }

        public async Task RollbackToCheckpointAsync(Guid instanceId)
        {
            using var lockScope = await _lockManager.AcquireLockAsync(
                $"workflow-rollback-{instanceId}",
                TimeSpan.FromSeconds(30)
            );

            // Get latest checkpoint
            var checkpoint = await _dbContext.WorkflowCheckpoints
                .Where(c => c.InstanceId == instanceId)
                .OrderByDescending(c => c.CheckpointTime)
                .FirstOrDefaultAsync();

            if (checkpoint == null)
            {
                throw new WorkflowException($"No checkpoint found for workflow instance {instanceId}");
            }

            var workflowData = await _dbContext.WorkflowDatas
                .Include(w => w.StepInstances)
                .FirstOrDefaultAsync(w => w.Id == instanceId);

            if (workflowData == null)
            {
                throw new WorkflowNotFoundException(message: $"Workflow instance {instanceId} not found");
            }

            // Rollback to checkpoint state
            workflowData.Status = checkpoint.Status;
            workflowData.Variables = checkpoint.Variables;
            workflowData.CurrentStepId = checkpoint.CurrentStepId;

            foreach (var (stepId, variables) in checkpoint.StepData)
            {
                var step = workflowData.StepInstances.FirstOrDefault(s => s.StepId == stepId);
                if (step != null)
                {
                    step.StepVariables = variables;
                }
            }

            workflowData.AuditLog += $"{DateTime.UtcNow:s} - Rolled back to checkpoint from {checkpoint.CheckpointTime:s}\n";
            await _dbContext.SaveChangesAsync();

            // Clear cache
            var cacheKey = $"workflow-state-{instanceId}";
            await _cache.RemoveAsync(cacheKey);

            _logger.LogInformation(
                "Rolled back workflow instance {InstanceId} to checkpoint at {CheckpointTime}",
                instanceId,
                checkpoint.CheckpointTime);
        }
    }
}
