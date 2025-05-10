using AppCommon.DTOs;
using AppCommon.DTOs.Modules;
using Application.Features.WorkFlow.Command;
using AppWorkflow.Common.DTO;
using AppWorkflow.Core.Domain.Data;
using AppWorkflow.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [Route("api/workflow/recovery")]
    [ApiController]
    public class WorkflowRecoveryController : ControllerBase
    {
        private readonly IWorkflowRecoveryService _recoveryService;
        private readonly ILogger<WorkflowRecoveryController> _logger;

        public WorkflowRecoveryController(
            IWorkflowRecoveryService recoveryService,
            ILogger<WorkflowRecoveryController> logger)
        {
            _recoveryService = recoveryService;
            _logger = logger;
        }

        [HttpPost("retry/{instanceId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<WorkflowCommandResult>>> RetryFailedWorkflow(Guid instanceId)
        {
            try
            {
                _logger.LogInformation("Retrying failed workflow instance {InstanceId}", instanceId);
                
                var canRecover = await _recoveryService.CanRecoverWorkflowAsync(instanceId);
                if (!canRecover)
                {
                    return ApiResponse<WorkflowCommandResult>.Fail(
                        "RECOVERY_ERROR",
                        $"Workflow instance {instanceId} cannot be recovered");
                }
                
                var result = await _recoveryService.RecoverWorkflowAsync(instanceId, RecoveryStrategy.RetryCurrentStep);
                
                return ApiResponse<WorkflowCommandResult>.Success(new WorkflowCommandResult
                {
                    Success = true,
                    WorkflowInstanceId = result.Id,
                    Message = "Workflow successfully retried"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrying workflow instance {InstanceId}", instanceId);
                return ApiResponse<WorkflowCommandResult>.Fail(
                    "RECOVERY_ERROR",
                    $"Error retrying workflow: {ex.Message}");
            }
        }

        [HttpGet("checkpoint/{instanceId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<Dictionary<Guid, Dictionary<string, object>>>>> GetCheckpointData(Guid instanceId)
        {
            try
            {
                _logger.LogInformation("Getting checkpoint data for workflow instance {InstanceId}", instanceId);
                
                var checkpointData = await _recoveryService.GetLatestCheckpointAsync(instanceId);
                
                return ApiResponse<Dictionary<Guid, Dictionary<string, object>>>.Success(checkpointData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting checkpoint data for workflow instance {InstanceId}", instanceId);
                return ApiResponse<Dictionary<Guid, Dictionary<string, object>>>.Fail(
                    "RECOVERY_ERROR",
                    $"Error getting checkpoint data: {ex.Message}");
            }
        }

        [HttpPost("restore/{instanceId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<bool>>> RestoreFromCheckpoint(
            Guid instanceId, 
            [FromBody] Dictionary<Guid, Dictionary<string, object>> checkpointData)
        {
            try
            {
                _logger.LogInformation("Restoring workflow instance {InstanceId} from provided checkpoint data", instanceId);
                
                // Create a checkpoint with the provided data
                await _recoveryService.CreateCheckpointAsync(instanceId, checkpointData.Values.FirstOrDefault() ?? new Dictionary<string, object>());
                
                // Recover the workflow using the latest checkpoint (the one we just created)
                var result = await _recoveryService.RecoverWorkflowAsync(instanceId, RecoveryStrategy.RestartFromLastCheckpoint);
                
                return ApiResponse<bool>.Success(result != null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error restoring workflow instance {InstanceId} from checkpoint data", instanceId);
                return ApiResponse<bool>.Fail(
                    "RECOVERY_ERROR",
                    $"Error restoring from checkpoint: {ex.Message}");
            }
        }

        [HttpGet("checkpoints/{instanceId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<IEnumerable<CheckpointInfoDto>>>> GetCheckpoints(Guid instanceId)
        {
            try
            {
                _logger.LogInformation("Getting checkpoints for workflow instance {InstanceId}", instanceId);
                
                var checkpoints = await _recoveryService.GetWorkflowCheckpointsAsync(instanceId);
                
                var checkpointDtos = checkpoints.Select(c => new CheckpointInfoDto
                {
                    CheckpointId = c.Id,
                    InstanceId = c.InstanceId,
                    CreatedAt = c.CheckpointTime,
                    StepId = c.CurrentStepId
                }).ToList();
                
                return ApiResponse<IEnumerable<CheckpointInfoDto>>.Success(checkpointDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting checkpoints for workflow instance {InstanceId}", instanceId);
                return ApiResponse<IEnumerable<CheckpointInfoDto>>.Fail(
                    "RECOVERY_ERROR",
                    $"Error getting checkpoints: {ex.Message}");
            }
        }

        [HttpPost("restore-checkpoint/{checkpointId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<bool>>> RestoreFromCheckpoint(Guid checkpointId)
        {
            try
            {
                _logger.LogInformation("Restoring workflow from checkpoint {CheckpointId}", checkpointId);
                
                // Get the checkpoint
                var checkpoints = (await _recoveryService.GetWorkflowCheckpointsAsync(Guid.Empty)).ToList();
                var checkpoint = checkpoints.FirstOrDefault(c => c.Id == checkpointId);
                
                if (checkpoint == null)
                {
                    return ApiResponse<bool>.Fail(
                        "NOT_FOUND",
                        $"Checkpoint with ID {checkpointId} not found");
                }
                
                // Restore from the checkpoint
                var result = await _recoveryService.RestoreFromCheckpointAsync(checkpoint.InstanceId, checkpointId);
                
                return ApiResponse<bool>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error restoring workflow from checkpoint {CheckpointId}", checkpointId);
                return ApiResponse<bool>.Fail(
                    "RECOVERY_ERROR",
                    $"Error restoring from checkpoint: {ex.Message}");
            }
        }

        [HttpGet("diagnostics/{instanceId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<WorkflowDiagnosticsDto>>> GetDiagnostics(Guid instanceId)
        {
            try
            {
                _logger.LogInformation("Getting diagnostic information for workflow instance {InstanceId}", instanceId);
                
                // Get information about the workflow instance and its checkpoints
                var canRecover = await _recoveryService.CanRecoverWorkflowAsync(instanceId);
                var checkpoints = await _recoveryService.GetWorkflowCheckpointsAsync(instanceId);
                
                var diagnostics = new WorkflowDiagnosticsDto
                {
                    InstanceId = instanceId,
                    RecoveryPossible = canRecover,
                    CheckpointCount = checkpoints.Count(),
                    LatestCheckpoint = checkpoints.OrderByDescending(c => c.CheckpointTime).FirstOrDefault()?.CheckpointTime,
                    AvailableRecoveryStrategies = canRecover
                        ? new List<string>
                        {
                            RecoveryStrategy.RestartFromLastCheckpoint.ToString(),
                            RecoveryStrategy.RetryCurrentStep.ToString(),
                            RecoveryStrategy.SkipCurrentStep.ToString()
                        }
                        : new List<string>()
                };
                
                return ApiResponse<WorkflowDiagnosticsDto>.Success(diagnostics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting diagnostic information for workflow instance {InstanceId}", instanceId);
                return ApiResponse<WorkflowDiagnosticsDto>.Fail(
                    "RECOVERY_ERROR",
                    $"Error getting diagnostics: {ex.Message}");
            }
        }
    }

    // DTOs to support the controller
    public class CheckpointInfoDto
    {
        public Guid CheckpointId { get; set; }
        public Guid InstanceId { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid StepId { get; set; }
    }

    public class WorkflowDiagnosticsDto
    {
        public Guid InstanceId { get; set; }
        public bool RecoveryPossible { get; set; }
        public int CheckpointCount { get; set; }
        public DateTime? LatestCheckpoint { get; set; }
        public List<string> AvailableRecoveryStrategies { get; set; } = new List<string>();
        public Dictionary<string, string> AdditionalInfo { get; set; } = new Dictionary<string, string>();
    }
}
