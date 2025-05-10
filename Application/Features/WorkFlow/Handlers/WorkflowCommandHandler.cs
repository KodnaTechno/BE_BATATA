using AppCommon.DTOs;
using AppCommon.DTOs.Modules;
using Application.Common.Handlers;
using Application.Features.WorkFlow.Command;
using Application.Services.EventsLogger;

using AppWorkflow.Core.Interfaces.Services;
using AppWorkflow.Core.Models;
using AppWorkflow.Infrastructure.Repositories.IRepository;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using WorkflowCommandResultApp = Application.Features.WorkFlow.Command.WorkflowCommandResult;
using AppWorkflow.Domain.Schema;
using AppWorkflow.Common.Enums;

namespace Application.Features.WorkFlow.Handlers
{
    public class WorkflowCommandHandler : BaseCommandHandler<WorkflowCommand, WorkflowCommandResultApp>
    {
        private readonly IWorkflowEngine _workflowEngine;
        private readonly IWorkflowRepository _workflowRepository;

        public WorkflowCommandHandler(
            IMediator mediator,
            ILogger<BaseCommandHandler<WorkflowCommand, WorkflowCommandResultApp>> logger,
            IEventLogger eventLogger,
            IHttpContextAccessor httpContextAccessor,
            IWorkflowEngine workflowEngine,
            IWorkflowRepository workflowRepository)
            : base(mediator, logger, eventLogger, httpContextAccessor)
        {
            _workflowEngine = workflowEngine;
            _workflowRepository = workflowRepository;
        }

        protected override async Task<ApiResponse<WorkflowCommandResultApp>> HandleCommand(
            WorkflowCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // 1. Find the workflow for this action
                var workflow = await FindWorkflowForActionAsync(request.ActionId, cancellationToken);
                if (workflow == null)
                {
                    return ApiResponse<WorkflowCommandResultApp>.Fail(
                        "WORKFLOW_NOT_FOUND",
                        $"No workflow found for action {request.ActionId}");
                }

                // 2. Create module data from parameters
                var moduleData = new WorkflowModuleData
                {
                    ModuleType = request.ModuleType,
                    ModuleProperties = request.PropertiesData
                };

                // 3. Start workflow execution
                var workflowInstance = await _workflowEngine.StartWorkflowAsync(
                    workflow.Id,
                    moduleData); // Pass parameters as initial variables

                // 4. Check if workflow completed synchronously or needs to run in background
                if (workflowInstance.Status == WorkflowStatus.Completed)
                {
                    // Workflow completed - retrieve result from output variables
                    return ApiResponse<WorkflowCommandResultApp>.Success(new WorkflowCommandResultApp
                    {
                        Success = true,
                        Result = ExtractPrimaryResult(workflowInstance.Variables),
                        OutputVariables = workflowInstance.Variables,
                        Message = "Workflow completed successfully"
                    });
                }


                // 5. Workflow is running in the background - return reference to track it
                return ApiResponse<WorkflowCommandResultApp>.Success(new WorkflowCommandResultApp
                {
                    Success = true,
                    Result = new { WorkflowId = workflowInstance.Id },
                    Message = "Workflow started successfully and is running in the background"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing workflow for action {ActionId}", request.ActionId);
                return ApiResponse<WorkflowCommandResultApp>.Fail(
                    "WORKFLOW_EXECUTION_ERROR",
                    $"Error executing workflow: {ex.Message}");
            }
        }

        private async Task<AppWorkflow.Domain.Schema.Workflow> FindWorkflowForActionAsync(Guid actionId, CancellationToken cancellationToken)
        {
            // Query to find workflow where the action ID is specified in metadata
            // Example:
            return await _workflowRepository.FindByActionIdAsync(actionId, cancellationToken);
        }


        private object ExtractPrimaryResult(Dictionary<string, object> variables)
        {
            // Extract the primary result from variables
            // This might be a specific variable like "result" or constructed from multiple variables
            return variables.TryGetValue("result", out var result) ? result : variables;
        }
    }
}
