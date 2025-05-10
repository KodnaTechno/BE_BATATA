using AppWorkflow.Common.DTO;

using AppWorkflow.Core.Interfaces.Services;
using AppWorkflow.Infrastructure.Data.Configurations;
using AppWorkflow.Infrastructure.Repositories.IRepository;
using AppWorkflow.Infrastructure.Triggers;
using AppWorkflow.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppWorkflow.Services
{
    public class WorkflowManagementService(
        IWorkflowRepository workflowRepository,
        IWorkflowDataRepository workflowDataRepository,
        IWorkflowValidator workflowValidator,
        IWorkflowVersionManager versionManager,
        IWorkflowEngine workflowEngine,
        IDistributedLockManager lockManager)
        : IWorkflowManagementService
    {
        private readonly IWorkflowDataRepository _workflowDataRepository = workflowDataRepository;
        private readonly IWorkflowVersionManager _versionManager = versionManager;
        private readonly IWorkflowEngine _workflowEngine = workflowEngine;

        public async Task<WorkflowDto> CreateWorkflowAsync(CreateWorkflowDto createDto, CancellationToken cancellationToken = default)
        {
            var workflow = new Workflow
            {
                Id = Guid.NewGuid(),
                Name = createDto.Name,
                Description = createDto.Description,
                CreatedAt = DateTime.UtcNow,
                Version = "1",
                Status = WorkflowStatus.Active,
                CreatedBy="dummy",
                Metadata = createDto.Metadata ?? new Dictionary<string, string>(),
                ModuleType="ttestt",
                RetryPolicy=new RetryPolicy(),
                TriggerConfigs=new List<TriggerConfiguration>(),
                UpdatedAt= DateTime.UtcNow,
                UpdatedBy="dummy",
                
            };

            // Map and connect steps
            var stepDict = new Dictionary<string, WorkflowStep>();
            workflow.Steps = createDto.Steps.Select(stepDto =>
            {
                var step = new WorkflowStep
                {
                    Id = Guid.NewGuid(),
                    Name = stepDto.Name,
                    ActionType = stepDto.Type,
                    CreatedBy="dummy",
                    UpdatedBy="dummi",
                    RetryPolicy = new RetryPolicy(),
                    Description =stepDto.Name,
                    ActionConfiguration = stepDto.Configuration ?? System.Text.Json.JsonDocument.Parse("{}"),
                };
                stepDict[step.Name] = step;
                return step;
            }).ToList();

          
            // Validate workflow
            var validationResult = await workflowValidator.ValidateWorkflowAsync(workflow);
            if (!validationResult.IsValid)
            {
                // Convert validation errors to a single string for the exception
                var errorMessage = string.Join("; ", validationResult.Errors.Select(e => e.Error));
                throw new WorkflowValidationException(errorMessage);
            }
            workflow.InitialStepId = workflow.Steps.FirstOrDefault()?.Id ?? Guid.Empty;
            // Create workflow and version
            await workflowRepository.CreateAsync(workflow, cancellationToken);

            return MapToDto(workflow);
        }

        public async Task<WorkflowDto> UpdateWorkflowAsync(Guid workflowId, UpdateWorkflowDto updateDto, CancellationToken cancellationToken = default)
        {
            var lockKey = $"workflow:{workflowId}";

            try
            {
                await lockManager.AcquireLockAsync(lockKey, TimeSpan.FromMinutes(5));

                var existingWorkflow = await workflowRepository.GetByIdAsync(workflowId)
                    ?? throw new WorkflowNotFoundException(workflowId);

                // Update basic properties
                existingWorkflow.Name = updateDto.Name;
                existingWorkflow.Description = updateDto.Description;
                existingWorkflow.UpdatedAt = DateTime.UtcNow;
                existingWorkflow.Version += 1;
                existingWorkflow.Metadata = updateDto.Metadata ?? existingWorkflow.Metadata;

               
                await workflowRepository.UpdateAsync(existingWorkflow);

                return MapToDto(existingWorkflow);
            }
            finally
            {
                await lockManager.ReleaseLockAsync(lockKey);
            }
        }

        public async Task<WorkflowDto> GetWorkflowAsync(Guid workflowId, CancellationToken cancellationToken = default)
        {
            var workflow = await workflowRepository.GetByIdAsync(workflowId)
                ?? throw new WorkflowNotFoundException(workflowId);

            return MapToDto(workflow);
        }

        public async Task<IEnumerable<WorkflowListItemDto>> GetWorkflowsAsync(
            int page = 1,
            int pageSize = 20,
            string? searchTerm = null,
            WorkflowStatus? status = null,
            CancellationToken cancellationToken = default)
        {
            // No GetAllAsync, so get all workflows and filter/paginate in memory
            var allWorkflows = await workflowRepository.GetVersionHistoryAsync(Guid.Empty); // Replace Guid.Empty with actual logic if needed
            var filtered = allWorkflows
                .Where(w => (string.IsNullOrEmpty(searchTerm) || (w.Name != null && w.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)))
                    && (!status.HasValue || w.Status == status.Value))
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(MapToListItemDto);
            return filtered;
        }

        public async Task DeleteWorkflowAsync(Guid workflowId, CancellationToken cancellationToken = default)
        {
            await workflowRepository.DeleteAsync(workflowId);
        }

        public async Task<WorkflowDto> PublishWorkflowAsync(Guid workflowId, CancellationToken cancellationToken = default)
        {
            var workflow = await workflowRepository.GetByIdAsync(workflowId);
            if (workflow == null) throw new WorkflowNotFoundException(workflowId);
            workflow.Status = WorkflowStatus.Active;
            await workflowRepository.UpdateAsync(workflow);
            return MapToDto(workflow);
        }

        public async Task<WorkflowDto> DeactivateWorkflowAsync(Guid workflowId, CancellationToken cancellationToken = default)
        {
            var workflow = await workflowRepository.GetByIdAsync(workflowId);
            if (workflow == null) throw new WorkflowNotFoundException(workflowId);
            workflow.Status = WorkflowStatus.Suspended;
            await workflowRepository.UpdateAsync(workflow);
            return MapToDto(workflow);
        }

        public async Task<IEnumerable<WorkflowDto>> GetWorkflowVersionHistoryAsync(Guid workflowId, CancellationToken cancellationToken = default)
        {
            var versions = await workflowRepository.GetVersionHistoryAsync(workflowId);
            return versions.Select(MapToDto);
        }

        public async Task<WorkflowDto> CloneWorkflowAsync(Guid workflowId, CancellationToken cancellationToken = default)
        {
            var workflow = await workflowRepository.GetByIdAsync(workflowId);
            if (workflow == null) throw new WorkflowNotFoundException(workflowId);
            var clone = new Workflow
            {
                Id = Guid.NewGuid(),
                Name = workflow.Name + " (Clone)",
                Description = workflow.Description,
                Status = WorkflowStatus.Draft,
                Version = "1",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CreatedBy = workflow.CreatedBy,
                UpdatedBy = workflow.UpdatedBy,
                Metadata = new Dictionary<string, string>(workflow.Metadata),
                ModuleType = workflow.ModuleType,
                RetryPolicy = workflow.RetryPolicy,
                TriggerConfigs = workflow.TriggerConfigs != null ? new List<TriggerConfiguration>(workflow.TriggerConfigs) : null,
                Steps = workflow.Steps.Select(s => new WorkflowStep
                {
                    Id = Guid.NewGuid(),
                    Name = s.Name,
                    ActionType = s.ActionType,
                    CreatedBy = s.CreatedBy,
                    UpdatedBy = s.UpdatedBy,
                    RetryPolicy = s.RetryPolicy,
                    Description = s.Description,
                    ActionConfiguration = s.ActionConfiguration,
                    Transitions = s.Transitions?.Select(t => new StepTransition
                    {
                        Id = Guid.NewGuid(),
                        TargetStepId = t.TargetStepId,
                        Condition = t.Condition
                    }).ToList()
                }).ToList(),
                Variables = workflow.Variables != null ? new List<WorkflowVariable>(workflow.Variables) : new List<WorkflowVariable>(),
                PropertiesKeys = workflow.PropertiesKeys != null ? new List<string>(workflow.PropertiesKeys) : new List<string>()
            };
            await workflowRepository.CreateAsync(clone, cancellationToken);
            return MapToDto(clone);
        }

        private WorkflowDto MapToDto(Workflow workflow)
        {
            return new WorkflowDto
            {
                Id = workflow.Id,
                Name = workflow.Name ?? string.Empty,
                Description = workflow.Description ?? string.Empty,
                Status = workflow.Status,
                Version = workflow.Version ?? string.Empty,
                CreatedAt = workflow.CreatedAt,
                UpdatedAt = workflow.UpdatedAt,
                Metadata = workflow.Metadata,
                Steps = workflow.Steps.Select(s => new WorkflowStepDto
                {
                    Id = s.Id,
                    Name = s.Name ?? string.Empty,
                    Transitions = s.Transitions?.Select(t => new StepTransitionDto
                    {
                        TargetStepId = t.TargetStepId,
                        Condition = t.Condition ?? string.Empty
                    }).ToList()
                }).ToList(),
             
              
            };
        }

        private WorkflowListItemDto MapToListItemDto(Workflow workflow)
        {
            return new WorkflowListItemDto
            {
                Id = workflow.Id,
                Name = workflow.Name ?? string.Empty,
                Description = workflow.Description ?? string.Empty,
                Status = workflow.Status,
                Version = workflow.Version ?? string.Empty,
                CreatedAt = workflow.CreatedAt,
                UpdatedAt = workflow.UpdatedAt
            };
        }

     
    }
}
