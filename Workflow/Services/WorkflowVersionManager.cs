using AppCommon.GlobalHelpers;
using AppWorkflow.Core.Domain.Schema;
using AppWorkflow.Core.Interfaces.Services;
using AppWorkflow.Infrastructure.Repositories.IRepository;

namespace AppWorkflow.Services
{
    public class WorkflowVersionManager : IWorkflowVersionManager
    {
        private readonly IWorkflowRepository _workflowRepository;
        private readonly ILogger<WorkflowVersionManager> _logger;
        private readonly IAuditLogService _auditLogService;
        private readonly IWorkflowValidator _validator;

        public WorkflowVersionManager(
            IWorkflowRepository workflowRepository,
            ILogger<WorkflowVersionManager> logger,
            IAuditLogService auditLogService,
            IWorkflowValidator validator)
        {
            _workflowRepository = workflowRepository;
            _logger = logger;
            _auditLogService = auditLogService;
            _validator = validator;
        }

        public async Task<Workflow> CreateNewVersionAsync(Guid workflowId)
        {
            var currentWorkflow = await _workflowRepository.GetByIdAsync(workflowId);
            if (currentWorkflow == null)
                throw new WorkflowNotFoundException(workflowId);

            try
            {
                // Create new version
                var newVersion = IncrementVersion(currentWorkflow.Version);
                var newWorkflow = CloneWorkflow(currentWorkflow, newVersion);

                // Mark old version as not latest
                currentWorkflow.IsLatestVersion = false;
                await _workflowRepository.UpdateAsync(currentWorkflow);

                // Save new version
                await _workflowRepository.CreateAsync(newWorkflow, default);

                // Log version creation
                //await _auditLogService.LogAsync(new AuditLogEntry
                //{
                //    EntityType = "Workflow",
                //    EntityId = newWorkflow.Id,
                //    Action = AuditAction.Created,
                //    NewValues = JsonSerializer.SerializeToDocument(new
                //    {
                //        Version = newVersion,
                //        BasedOn = currentWorkflow.Version
                //    })
                //});

                return newWorkflow;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create new version for workflow {WorkflowId}", workflowId);
                throw new WorkflowEngineException("Failed to create new version", workflowId, null, null, ex);
            }
        }

        public async Task<Workflow> GetLatestVersionAsync(Guid workflowId)
        {
            return await _workflowRepository.GetLatestVersionAsync(workflowId);
        }

        public async Task<IEnumerable<Workflow>> GetVersionHistoryAsync(Guid workflowId)
        {
            return await _workflowRepository.GetVersionHistoryAsync(workflowId);
        }

        public async Task<bool> IsLatestVersionAsync(Guid workflowId, string version)
        {
            var latest = await GetLatestVersionAsync(workflowId);
            return latest.Version == version;
        }

        private string IncrementVersion(string currentVersion)
        {
            var versionParts = currentVersion.Split('.');
            if (versionParts.Length != 3)
                throw new WorkflowException($"Invalid version format: {currentVersion}");

            if (!int.TryParse(versionParts[2], out int patch))
                throw new WorkflowException($"Invalid patch version: {versionParts[2]}");

            return $"{versionParts[0]}.{versionParts[1]}.{patch + 1}";
        }

        private Workflow CloneWorkflow(Workflow source, string newVersion)
        {
            var clone = new Workflow
            {
                Id = Guid.NewGuid(),
                Name = source.Name,
                Description = source.Description,
                ModuleType = source.ModuleType,
                Status = WorkflowStatus.Draft,
                Version = newVersion,
                IsLatestVersion = true,
                InitialStepId = source.InitialStepId,
                Timeout = source.Timeout,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = source.UpdatedBy ?? source.CreatedBy
            };

            // Clone steps
            clone.Steps = source.Steps.Select(step => new WorkflowStep
            {
                Id = Guid.NewGuid(),
                WorkflowId = clone.Id,
                Name = step.Name,
                Description = step.Description,
                ActionType = step.ActionType,
                ActionConfiguration = step.ActionConfiguration.Clone(),
                Status = StepStatus.Pending,
                Timeout = step.Timeout,
                IsParallel = step.IsParallel,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = clone.CreatedBy
            }).ToList();

            // Update transitions with new step IDs
            var stepIdMap = source.Steps.Zip(clone.Steps, (old, @new) =>
                new { OldId = old.Id, NewId = @new.Id }).ToDictionary(x => x.OldId, x => x.NewId);

            foreach (var step in clone.Steps)
            {
                var originalStep = source.Steps.First(s => s.Name == step.Name);
                step.Transitions = originalStep.Transitions.Select(t => new StepTransition
                {
                    SourceStepId = stepIdMap[t.SourceStepId],
                    TargetStepId = stepIdMap[t.TargetStepId],
                    Name = t.Name,
                    Description = t.Description,
                    Condition = t.Condition,
                    Priority = t.Priority
                }).ToList();

                if (step.IsParallel)
                {
                    step.ParallelSteps = originalStep.ParallelSteps
                        .Select(id => stepIdMap[id])
                        .ToList();
                }
            }

            // Clone variables and other collections
            clone.Variables = source.Variables.Select(v => new WorkflowVariable
            {
                Name = v.Name,
                Description = v.Description,
                Type = v.Type,
                DefaultValue = v.DefaultValue,
                IsRequired = v.IsRequired,
                ValidationExpression = v.ValidationExpression
            }).ToList();

            clone.TriggerConfigs = source.TriggerConfigs.CloneObj();
            clone.Metadata = new Dictionary<string, string>(source.Metadata);

            return clone;
        }
    }
}
