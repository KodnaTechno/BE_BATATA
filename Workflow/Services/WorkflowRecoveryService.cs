using AppWorkflow.Common.Enums;
using AppWorkflow.Core.Domain.Data;
using AppWorkflow.Core.Interfaces.Services;
using AppWorkflow.Infrastructure.Repositories.IRepository;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppWorkflow.Services
{
    public interface IWorkflowRecoveryService
    {
        Task<bool> CreateCheckpointAsync(Guid instanceId, Dictionary<string, object> checkpointData);
        Task<Dictionary<Guid, Dictionary<string, object>>> GetLatestCheckpointAsync(Guid instanceId);
        Task<bool> RestoreFromCheckpointAsync(Guid instanceId, Guid checkpointId);
        Task<bool> CanRecoverWorkflowAsync(Guid instanceId);
        Task<WorkflowData> RecoverWorkflowAsync(Guid instanceId, RecoveryStrategy strategy);
        Task<IEnumerable<WorkflowCheckpoint>> GetWorkflowCheckpointsAsync(Guid instanceId);
    }

    public enum RecoveryStrategy
    {
        RestartFromLastCheckpoint,
        RestartFromBeginning,
        RetryCurrentStep,
        SkipCurrentStep,
        UseCompensatingActions
    }

    public class WorkflowRecoveryService : IWorkflowRecoveryService
    {
        private readonly IWorkflowEngine _workflowEngine;
        private readonly IWorkflowDataRepository _instanceRepository;
        private readonly IWorkflowCheckpointRepository _checkpointRepository;
        private readonly ILogger<WorkflowRecoveryService> _logger;

        public WorkflowRecoveryService(
            IWorkflowEngine workflowEngine,
            IWorkflowDataRepository instanceRepository,
            IWorkflowCheckpointRepository checkpointRepository,
            ILogger<WorkflowRecoveryService> logger)
        {
            _workflowEngine = workflowEngine;
            _instanceRepository = instanceRepository;
            _checkpointRepository = checkpointRepository;
            _logger = logger;
        }

        public async Task<bool> CreateCheckpointAsync(Guid instanceId, Dictionary<string, object> checkpointData)
        {
            try
            {
                var instance = await _instanceRepository.GetByIdAsync(instanceId);
                if (instance == null)
                {
                    _logger.LogError("Cannot create checkpoint for non-existent workflow instance {InstanceId}", instanceId);
                    return false;
                }

                var checkpoint = new WorkflowCheckpoint
                {
                    Id = Guid.NewGuid(),
                    InstanceId = instanceId,
                    CheckpointTime = DateTime.UtcNow,
                    Variables = new Dictionary<string, object>(instance.Variables),
                    CurrentStepId = instance.CurrentStepId,
                    StepData = new() { { instanceId, checkpointData } },
                    CreatedBy = "System"
                };

                await _checkpointRepository.CreateAsync(checkpoint);
                _logger.LogInformation("Created checkpoint {CheckpointId} for workflow instance {InstanceId}", checkpoint.Id, instanceId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating checkpoint for workflow instance {InstanceId}", instanceId);
                return false;
            }
        }

        public async Task<Dictionary<Guid, Dictionary<string, object>>> GetLatestCheckpointAsync(Guid instanceId)
        {
            var checkpoints = await _checkpointRepository.GetCheckpointsForInstanceAsync(instanceId);
            var latestCheckpoint = checkpoints.OrderByDescending(c => c.CheckpointTime).FirstOrDefault();
            return latestCheckpoint?.StepData ?? new Dictionary<Guid, Dictionary<string, object>>();
        }

        public async Task<bool> RestoreFromCheckpointAsync(Guid instanceId, Guid checkpointId)
        {
            try
            {
                var instance = await _instanceRepository.GetByIdAsync(instanceId);
                if (instance == null)
                {
                    _logger.LogError("Cannot restore non-existent workflow instance {InstanceId}", instanceId);
                    return false;
                }

                var checkpoint = await _checkpointRepository.GetByIdAsync(checkpointId);
                if (checkpoint == null || checkpoint.InstanceId != instanceId)
                {
                    _logger.LogError("Checkpoint {CheckpointId} not found for workflow instance {InstanceId}", checkpointId, instanceId);
                    return false;
                }

                // Restore workflow state from checkpoint
                instance.Variables = new Dictionary<string, object>(checkpoint.Variables);
                instance.CurrentStepId = checkpoint.CurrentStepId;
                instance.Status = WorkflowStatus.Active;
                instance.UpdatedAt = DateTime.UtcNow;

                await _instanceRepository.UpdateAsync(instance);
                _logger.LogInformation("Restored workflow instance {InstanceId} from checkpoint {CheckpointId}", instanceId, checkpointId);
                
                // Continue workflow execution from restored step
                await _workflowEngine.ExecuteStepAsync(instanceId, checkpoint.CurrentStepId);
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error restoring workflow instance {InstanceId} from checkpoint {CheckpointId}", instanceId, checkpointId);
                return false;
            }
        }

        public async Task<bool> CanRecoverWorkflowAsync(Guid instanceId)
        {
            var instance = await _instanceRepository.GetByIdAsync(instanceId);
            if (instance == null) return false;
            
            // Check if workflow has failed or is in error state
            if (instance.Status != WorkflowStatus.Failed )
            {
                return false;
            }
            
            // Check if there are any checkpoints or if retry is possible based on workflow settings
            var checkpoints = await _checkpointRepository.GetCheckpointsForInstanceAsync(instanceId);
            return checkpoints.Any() || instance.StepInstances.Any(s => s.Status == StepStatus.Failed && s.RetryCount < 3);
        }

        public async Task<WorkflowData> RecoverWorkflowAsync(Guid instanceId, RecoveryStrategy strategy)
        {
            var instance = await _instanceRepository.GetByIdAsync(instanceId);
            if (instance == null)
            {
                _logger.LogError("Cannot recover non-existent workflow instance {InstanceId}", instanceId);
                return null;
            }

            switch (strategy)
            {
                case RecoveryStrategy.RestartFromLastCheckpoint:
                    var checkpoints = await _checkpointRepository.GetCheckpointsForInstanceAsync(instanceId);
                    var latestCheckpoint = checkpoints.OrderByDescending(c => c.CheckpointTime).FirstOrDefault();
                    if (latestCheckpoint != null)
                    {
                        await RestoreFromCheckpointAsync(instanceId, latestCheckpoint.Id);
                    }
                    break;

                case RecoveryStrategy.RestartFromBeginning:
                    // Reset the workflow and restart from the beginning
                    instance.CurrentStepId = instance.WorkflowId; // Assuming initial step
                    instance.Variables = new Dictionary<string, object>();
                    instance.Status = WorkflowStatus.Active;
                    await _instanceRepository.UpdateAsync(instance);
                    await _workflowEngine.ExecuteStepAsync(instanceId, instance.CurrentStepId);
                    break;

                case RecoveryStrategy.RetryCurrentStep:
                    // Retry the current step
                    await _workflowEngine.RetryStepAsync(instanceId, instance.CurrentStepId);
                    break;

                case RecoveryStrategy.SkipCurrentStep:
                    // Skip the current step and proceed to the next
                    await _workflowEngine.SkipStepAsync(instanceId, instance.CurrentStepId);
                    break;

                case RecoveryStrategy.UseCompensatingActions:
                    // Apply compensating actions to rollback partial changes
                    // This requires custom logic based on your workflow definitions
                    // and would typically involve executing specific "undo" steps
                    _logger.LogWarning("Compensating actions for workflow {InstanceId} are not implemented", instanceId);
                    break;
            }

            // Return the updated instance
            return await _instanceRepository.GetByIdAsync(instanceId);
        }

        public async Task<IEnumerable<WorkflowCheckpoint>> GetWorkflowCheckpointsAsync(Guid instanceId)
        {
            return await _checkpointRepository.GetCheckpointsForInstanceAsync(instanceId);
        }
    }
}
