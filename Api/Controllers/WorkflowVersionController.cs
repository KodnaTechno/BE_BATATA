using AppCommon.DTOs;
using AppWorkflow.Common.DTO;
using AppWorkflow.Core.Domain.Data;
using AppWorkflow.Core.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [Route("api/workflow/version")]
    [ApiController]
    public class WorkflowVersionController : ControllerBase
    {
        private readonly IWorkflowVersionManager _versionManager;
        private readonly IWorkflowMigrationService _migrationService;
        private readonly ILogger<WorkflowVersionController> _logger;

        public WorkflowVersionController(
            IWorkflowVersionManager versionManager,
            IWorkflowMigrationService migrationService,
            ILogger<WorkflowVersionController> logger)
        {
            _versionManager = versionManager;
            _migrationService = migrationService;
            _logger = logger;
        }        [HttpPost("{workflowId}/create-version")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<WorkflowVersionInfoDto>>> CreateNewVersion(Guid workflowId)
        {
            try
            {
                // Create the new version
                var newVersion = await _versionManager.CreateNewVersionAsync(workflowId);
                if (newVersion == null)
                {
                    return ApiResponse<WorkflowVersionInfoDto>.Fail(
                        "VERSION_CREATION_ERROR",
                        $"Failed to create new version for workflow {workflowId}");
                }
                
                // Return the new version info
                var versionInfo = new WorkflowVersionInfoDto
                {
                    WorkflowId = newVersion.Id,
                    Version = newVersion.Version,
                    IsLatest = newVersion.IsLatestVersion,
                    CreatedAt = newVersion.CreatedAt,
                    CreatedBy = newVersion.CreatedBy,
                    ActiveInstances = 0  // New version has no instances yet
                };
                
                return ApiResponse<WorkflowVersionInfoDto>.Success(versionInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating new version for workflow {WorkflowId}", workflowId);
                return ApiResponse<WorkflowVersionInfoDto>.Fail(
                    "VERSION_CREATION_ERROR",
                    $"Error creating new version: {ex.Message}");
            }
        }[HttpGet("{workflowId}/latest")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<WorkflowVersionInfoDto>>> GetLatestVersion(Guid workflowId)
        {
            try
            {
                var latestVersion = await _versionManager.GetLatestVersionAsync(workflowId);
                if (latestVersion == null)
                {
                    return ApiResponse<WorkflowVersionInfoDto>.Fail(
                        "NOT_FOUND",
                        $"Workflow with ID {workflowId} not found");
                }
                
                // Get instance count for this version
                var instances = await _migrationService.GetInstancesForVersionAsync(latestVersion.Version);
                var activeInstances = instances.Count(i => i.WorkflowId == workflowId);
                
                var versionInfo = new WorkflowVersionInfoDto
                {
                    WorkflowId = latestVersion.Id,
                    Version = latestVersion.Version,
                    IsLatest = latestVersion.IsLatestVersion,
                    CreatedAt = latestVersion.CreatedAt,
                    CreatedBy = latestVersion.CreatedBy,
                    ActiveInstances = activeInstances
                };
                
                return ApiResponse<WorkflowVersionInfoDto>.Success(versionInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving latest version for workflow {WorkflowId}", workflowId);
                return ApiResponse<WorkflowVersionInfoDto>.Fail(
                    "VERSION_RETRIEVAL_ERROR",
                    $"Error retrieving latest version: {ex.Message}");
            }
        }[HttpGet("{workflowId}/history")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<IEnumerable<WorkflowVersionInfoDto>>>> GetVersionHistory(Guid workflowId)
        {
            try
            {
                var history = await _versionManager.GetVersionHistoryAsync(workflowId);
                
                if (history == null || !history.Any())
                {
                    return ApiResponse<IEnumerable<WorkflowVersionInfoDto>>.Fail(
                        "NOT_FOUND",
                        $"No version history found for workflow {workflowId}");
                }
                
                // Get instance information for each version
                var versionInfoList = new List<WorkflowVersionInfoDto>();
                
                foreach (var version in history)
                {
                    var instances = await _migrationService.GetInstancesForVersionAsync(version.Version);
                    var activeInstances = instances.Count(i => i.WorkflowId == workflowId);
                    
                    versionInfoList.Add(new WorkflowVersionInfoDto
                    {
                        WorkflowId = version.Id,
                        Version = version.Version,
                        IsLatest = version.IsLatestVersion,
                        CreatedAt = version.CreatedAt,
                        CreatedBy = version.CreatedBy,
                        ActiveInstances = activeInstances
                    });
                }
                
                return ApiResponse<IEnumerable<WorkflowVersionInfoDto>>.Success(versionInfoList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving version history for workflow {WorkflowId}", workflowId);
                return ApiResponse<IEnumerable<WorkflowVersionInfoDto>>.Fail(
                    "VERSION_HISTORY_ERROR",
                    $"Error retrieving version history: {ex.Message}");
            }
        }

        [HttpGet("{workflowId}/is-latest")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<bool>>> IsLatestVersion(
            Guid workflowId, [FromQuery] string version)
        {
            try
            {
                var isLatest = await _versionManager.IsLatestVersionAsync(workflowId, version);
                return ApiResponse<bool>.Success(isLatest);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if version {Version} is latest for workflow {WorkflowId}", 
                    version, workflowId);
                return ApiResponse<bool>.Fail(
                    "VERSION_CHECK_ERROR",
                    $"Error checking version: {ex.Message}");
            }
        }        [HttpPost("migrate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<WorkflowMigrationResultDto>>> MigrateInstance(
            [FromBody] WorkflowMigrationDto migrationRequest)
        {
            try
            {
                // Get the current workflow instance to determine the source version
                var workflowInstance = await _migrationService.GetInstancesForVersionAsync(migrationRequest.TargetVersion)
                    .ContinueWith(t => t.Result.FirstOrDefault(i => i.Id == migrationRequest.InstanceId));
                
                if (workflowInstance == null)
                {
                    return ApiResponse<WorkflowMigrationResultDto>.Fail(
                        "INSTANCE_NOT_FOUND",
                        $"Workflow instance with ID {migrationRequest.InstanceId} not found");
                }
                
                string sourceVersion = workflowInstance.WorkflowVersion;
                
                // Perform the actual migration
                var success = await _migrationService.MigrateInstanceAsync(
                    migrationRequest.InstanceId, 
                    migrationRequest.TargetVersion);
                
                if (success)
                {
                    // Create a rich response object
                    var result = new WorkflowMigrationResultDto
                    {
                        Success = true,
                        InstanceId = migrationRequest.InstanceId,
                        SourceVersion = sourceVersion,
                        TargetVersion = migrationRequest.TargetVersion,
                        MigrationStrategy = migrationRequest.MigrationStrategy,
                        MigrationTime = DateTime.UtcNow
                    };
                    
                    return ApiResponse<WorkflowMigrationResultDto>.Success(result);
                }
                else
                {
                    return ApiResponse<WorkflowMigrationResultDto>.Fail(
                        "MIGRATION_FAILED",
                        "The migration process failed to complete successfully");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error migrating instance {InstanceId} to version {TargetVersion}", 
                    migrationRequest.InstanceId, migrationRequest.TargetVersion);
                return ApiResponse<WorkflowMigrationResultDto>.Fail(
                    "MIGRATION_ERROR",
                    $"Error during migration: {ex.Message}");
            }
        }

        [HttpGet("validate-migration-path")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<bool>>> ValidateMigrationPath(
            [FromQuery] string sourceVersion, [FromQuery] string targetVersion)
        {
            try
            {
                await _migrationService.ValidateMigrationPathAsync(sourceVersion, targetVersion);
                return ApiResponse<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Invalid migration path from {SourceVersion} to {TargetVersion}", 
                    sourceVersion, targetVersion);
                return ApiResponse<bool>.Fail(
                    "INVALID_MIGRATION_PATH",
                    $"Invalid migration path: {ex.Message}");
            }
        }        [HttpGet("instances-for-version")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<List<object>>>> GetInstancesForVersion(
            [FromQuery] string version, [FromQuery] int maxResults = 100)
        {
            try
            {
                var instances = await _migrationService.GetInstancesForVersionAsync(version);
                
                // Create a simplified view for the API response
                var result = instances
                    .Take(maxResults)
                    .Select(i => new 
                    {
                        Id = i.Id,
                        WorkflowId = i.WorkflowId,
                        Status = i.Status,
                        CurrentStepId = i.CurrentStepId,
                        ModuleType = i.ModuleData?.ModuleType,
                        StartedAt = i.StartedAt,
                        CompletedAt = i.CompletedAt,
                        Duration = i.CompletedAt.HasValue ? (i.CompletedAt.Value - i.StartedAt).ToString() : null,
                        CreatedBy = i.CreatedBy,
                        HasError = !string.IsNullOrEmpty(i.ErrorDetails)
                    })
                    .ToList<object>();
                
                return ApiResponse<List<object>>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving instances for version {Version}", version);
                return ApiResponse<List<object>>.Fail(
                    "INSTANCES_RETRIEVAL_ERROR",
                    $"Error retrieving instances: {ex.Message}");
            }
        }
        
        [HttpGet("{workflowId}/version-metrics")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<object>>> GetVersionMetrics(Guid workflowId)
        {
            try
            {
                // Get all versions of the workflow
                var versions = await _versionManager.GetVersionHistoryAsync(workflowId);
                
                if (!versions.Any())
                {
                    return ApiResponse<object>.Fail(
                        "NOT_FOUND", 
                        $"No versions found for workflow {workflowId}");
                }
                
                var versionMetrics = new List<object>();
                
                foreach (var version in versions)
                {
                    // Get instances for this version
                    var instances = await _migrationService.GetInstancesForVersionAsync(version.Version);
                    var workflowInstances = instances.Where(i => i.WorkflowId == workflowId).ToList();
                    
                    // Calculate metrics
                    int totalInstances = workflowInstances.Count;
                    int activeInstances = workflowInstances.Count(i => i.Status == AppWorkflow.Common.Enums.WorkflowStatus.Active);
                    int completedInstances = workflowInstances.Count(i => i.Status == AppWorkflow.Common.Enums.WorkflowStatus.Completed);
                    int failedInstances = workflowInstances.Count(i => 
                        i.Status == AppWorkflow.Common.Enums.WorkflowStatus.Failed);
                    
                    // Calculate average completion time
                    double averageCompletionTime = 0;
                    var completedList = workflowInstances.Where(i => 
                        i.Status == AppWorkflow.Common.Enums.WorkflowStatus.Completed && 
                        i.CompletedAt.HasValue);
                        
                    if (completedList.Any())
                    {
                        averageCompletionTime = completedList
                            .Select(i => (i.CompletedAt.Value - i.StartedAt).TotalSeconds)
                            .Average();
                    }
                    
                    versionMetrics.Add(new
                    {
                        Version = version.Version,
                        IsLatest = version.IsLatestVersion,
                        CreatedAt = version.CreatedAt,
                        TotalInstances = totalInstances,
                        ActiveInstances = activeInstances,
                        CompletedInstances = completedInstances,
                        FailedInstances = failedInstances,
                        AverageCompletionTimeSeconds = averageCompletionTime
                    });
                }
                
                // Overall summary
                var result = new
                {
                    WorkflowId = workflowId,
                    TotalVersions = versions.Count(),
                    VersionDetails = versionMetrics.OrderByDescending(v => ((dynamic)v).CreatedAt)
                };
                
                return ApiResponse<object>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving version metrics for workflow {WorkflowId}", workflowId);
                return ApiResponse<object>.Fail(
                    "VERSION_METRICS_ERROR",
                    $"Error retrieving version metrics: {ex.Message}");
            }
        }
    }
}
