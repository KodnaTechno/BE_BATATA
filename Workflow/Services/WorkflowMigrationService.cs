using AppWorkflow.Core.Domain.Data;

using AppWorkflow.Core.Interfaces.Services;
using AppWorkflow.Infrastructure.Repositories.IRepository;
using AppWorkflow.Services.Interfaces;

namespace AppWorkflow.Services
{
    public class WorkflowMigrationService : IWorkflowMigrationService
    {
        private readonly IWorkflowRepository _workflowRepository;
        private readonly IWorkflowDataRepository _workflowDataRepository;
        private readonly IWorkflowStateManager _stateManager;
        private readonly ILogger<WorkflowMigrationService> _logger;
        private readonly IAuditLogService _auditLogService;
        private readonly IWorkflowValidator _validator;
        private readonly IDistributedLockManager _lockManager;

        public WorkflowMigrationService(
            IWorkflowRepository workflowRepository,
            IWorkflowStateManager stateManager,
            ILogger<WorkflowMigrationService> logger,
            IAuditLogService auditLogService,
            IWorkflowValidator validator,
            IDistributedLockManager lockManager,
            IWorkflowDataRepository workflowDataRepository)
        {
            _workflowRepository = workflowRepository;
            _stateManager = stateManager;
            _logger = logger;
            _auditLogService = auditLogService;
            _validator = validator;
            _lockManager = lockManager;
            _workflowDataRepository = workflowDataRepository;
        }

        public async Task<bool> MigrateInstanceAsync(Guid instanceId, string targetVersion)
        {
            using var lockScope = await _lockManager.AcquireLockAsync(
                $"workflow-migration-{instanceId}",
                TimeSpan.FromMinutes(5));

            try
            {
                // Get current instance
                var instance = await _workflowRepository.GetInstanceAsync(instanceId);
                if (instance == null)
                    throw new WorkflowNotFoundException(message: $"Instance {instanceId} not found");

                // Get source and target workflow versions
                var sourceWorkflow = await _workflowRepository.GetWorkflowVersionAsync(
                    instance.WorkflowId,
                    instance.WorkflowVersion);

                var targetWorkflow = await _workflowRepository.GetWorkflowVersionAsync(
                    instance.WorkflowId,
                    targetVersion);

                if (targetWorkflow == null)
                    throw new WorkflowException($"Target version {targetVersion} not found");

                // Validate migration path
                await ValidateMigrationPathAsync(instance.WorkflowVersion, targetVersion);

                // Create migration map
                var migrationMap = CreateMigrationMap(sourceWorkflow, targetWorkflow);

                // Save checkpoint before migration
                await _stateManager.CreateCheckpointAsync(instanceId);

                try
                {
                    // Update instance version
                    instance.WorkflowVersion = targetVersion;

                    // Migrate step instances
                    foreach (var stepInstance in instance.StepInstances)
                    {
                        if (migrationMap.TryGetValue(stepInstance.StepId, out var newStepId))
                        {
                            stepInstance.StepId = newStepId;
                        }
                        else
                        {
                            _logger.LogWarning(
                                "Step {StepId} from version {SourceVersion} has no mapping in version {TargetVersion}",
                                stepInstance.StepId,
                                instance.WorkflowVersion,
                                targetVersion);
                        }
                    }

                    // Update current step ID
                    if (migrationMap.TryGetValue(instance.CurrentStepId, out var newCurrentStepId))
                    {
                        instance.CurrentStepId = newCurrentStepId;
                    }

                    // Validate migrated instance
                    var validationResult = await ValidateMigratedInstanceAsync(instance, targetWorkflow);
                    if (!validationResult.IsValid)
                    {
                        throw new WorkflowValidationException(
                            "Migration validation failed",
                            validationResult.Errors.Select(e => e.Error));
                    }

                    // Save migrated instance
                    await _workflowDataRepository.UpdateInstanceAsync(instance);

                    // Log migration
                    //{
                    //    EntityType = "WorkflowInstance",
                    //    EntityId = instanceId,
                    //    Action = AuditAction.Updated,
                    //    NewValues = JsonSerializer.SerializeToDocument(new
                    //    {
                    //        FromVersion = sourceWorkflow.Version,
                    //        ToVersion = targetVersion,
                    //        MigrationMap = migrationMap
                    //    })
                    //});

                    return true;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "Error during migration of instance {InstanceId} to version {TargetVersion}",
                        instanceId,
                        targetVersion);

                    // Rollback to checkpoint
                    await _stateManager.RollbackToCheckpointAsync(instanceId);
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to migrate workflow instance {InstanceId} to version {TargetVersion}",
                    instanceId,
                    targetVersion);
                throw;
            }
        }

        public async Task<IEnumerable<WorkflowData>> GetInstancesForVersionAsync(string version)
        {
            return await _workflowDataRepository.GetInstancesByVersionAsync(version);
        }

        public async Task ValidateMigrationPathAsync(string sourceVersion, string targetVersion)
        {
            if (!IsValidVersion(sourceVersion) || !IsValidVersion(targetVersion))
            {
                throw new WorkflowValidationException(
                    "Invalid version format",
                    new[] { "Version must be in format X.Y.Z" });
            }

            var sourceParts = sourceVersion.Split('.').Select(int.Parse).ToArray();
            var targetParts = targetVersion.Split('.').Select(int.Parse).ToArray();

            // Major version changes require manual migration
            if (targetParts[0] != sourceParts[0])
            {
                throw new WorkflowValidationException(
                    "Cannot automatically migrate between major versions",
                    new[] { "Major version changes require manual migration" });
            }

            // Ensure target version is newer
            var sourceValue = (sourceParts[0] * 10000) + (sourceParts[1] * 100) + sourceParts[2];
            var targetValue = (targetParts[0] * 10000) + (targetParts[1] * 100) + targetParts[2];

            if (targetValue <= sourceValue)
            {
                throw new WorkflowValidationException(
                    "Invalid migration path",
                    new[] { "Target version must be newer than source version" });
            }
        }

        private Dictionary<Guid, Guid> CreateMigrationMap(Workflow sourceWorkflow, Workflow targetWorkflow)
        {
            var migrationMap = new Dictionary<Guid, Guid>();

            // Map steps based on name and configuration
            foreach (var sourceStep in sourceWorkflow.Steps)
            {
                var targetStep = targetWorkflow.Steps.FirstOrDefault(s =>
                    s.Name == sourceStep.Name &&
                    s.ActionType == sourceStep.ActionType);

                if (targetStep != null)
                {
                    migrationMap[sourceStep.Id] = targetStep.Id;
                }
            }

            return migrationMap;
        }

        private async Task<ValidationResult> ValidateMigratedInstanceAsync(
            WorkflowData instance,
            Workflow targetWorkflow)
        {
            var errors = new List<ValidationError>();

            // Validate current step exists in target version
            if (!targetWorkflow.Steps.Any(s => s.Id == instance.CurrentStepId))
            {
                errors.Add(new ValidationError
                {
                    Property = "CurrentStepId",
                    Error = "Current step does not exist in target version",
                    Severity = ValidationSeverity.Error,
                    Code = "INVALID_CURRENT_STEP"
                });
            }

            // Validate all step instances reference valid steps
            foreach (var stepInstance in instance.StepInstances)
            {
                if (!targetWorkflow.Steps.Any(s => s.Id == stepInstance.StepId))
                {
                    errors.Add(new ValidationError
                    {
                        Property = "StepInstance",
                        Error = $"Step instance references invalid step {stepInstance.StepId}",
                        Severity = ValidationSeverity.Error,
                        Code = "INVALID_STEP_REFERENCE"
                    });
                }
            }

            return new ValidationResult
            {
                IsValid = !errors.Any(),
                Errors = errors
            };
        }

        private bool IsValidVersion(string version)
        {
            if (string.IsNullOrEmpty(version)) return false;

            var parts = version.Split('.');
            if (parts.Length != 3) return false;

            return parts.All(p => int.TryParse(p, out _));
        }
    }
}
