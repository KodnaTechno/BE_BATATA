using AppWorkflow.Common.DTO;
using AppWorkflow.Core.Domain.Schema;
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
                AuditLog=string.Empty,
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
                    ActionConfiguration = stepDto.Configuration ?? default,
                };
                stepDict[step.Name] = step;
                return step;
            }).ToList();

          
            // Validate workflow
            var validationResult = await workflowValidator.ValidateWorkflowAsync(workflow);
            if (!validationResult.IsValid)
            {
                throw new WorkflowValidationException(validationResult.Errors);
            }
            workflow.InitialStepId = workflow.Steps.FirstOrDefault().Id;
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
            string searchTerm = null,
            WorkflowStatus? status = null,
            CancellationToken cancellationToken = default)
        {
           throw new NotImplementedException();
        }

        // ... Other methods (delete, publish, etc.) with DTO mapping

        private WorkflowDto MapToDto(Workflow workflow)
        {
            return new WorkflowDto
            {
                Id = workflow.Id,
                Name = workflow.Name,
                Description = workflow.Description,
                Status = workflow.Status,
                Version = workflow.Version,
                CreatedAt = workflow.CreatedAt,
                UpdatedAt = workflow.UpdatedAt,
                Metadata = workflow.Metadata,
                Steps = workflow.Steps.Select(s => new WorkflowStepDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    Transitions = s.Transitions?.Select(t => new StepTransitionDto
                    {
                        TargetStepId = t.TargetStepId,
                        Condition = t.Condition
                    }).ToList()
                }).ToList(),
             
              
            };
        }

        private WorkflowListItemDto MapToListItemDto(Workflow workflow)
        {
            return new WorkflowListItemDto
            {
                Id = workflow.Id,
                Name = workflow.Name,
                Description = workflow.Description,
                Status = workflow.Status,
                Version = workflow.Version,
                CreatedAt = workflow.CreatedAt,
                UpdatedAt = workflow.UpdatedAt
            };
        }

       

     
    }
}
