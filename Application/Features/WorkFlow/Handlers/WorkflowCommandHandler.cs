﻿using AppCommon.DTOs;
using AppCommon.DTOs.Modules;
using Application.Common.Handlers;
using Application.Features.WorkFlow.Command;
using Application.Services.EventsLogger;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

using AppWorkflow.Core.Interfaces.Services;
using AppWorkflow.Core.Models;
using AppWorkflow.Infrastructure.Repositories.IRepository;
using AppWorkflow.Services;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using WorkflowCommandResultApp = Application.Features.WorkFlow.Command.WorkflowCommandResult;
using AppWorkflow.Domain.Schema;
using AppWorkflow.Common.Enums;
using AppWorkflow.Core.Domain.Data;

namespace Application.Features.WorkFlow.Handlers
{
    public class WorkflowCommandHandler : BaseCommandHandler<WorkflowCommand, WorkflowCommandResultApp>
    {
        private readonly IWorkflowEngine _workflowEngine;
        private readonly IWorkflowRepository _workflowRepository;
        private readonly IWorkflowRecoveryService _recoveryService;

        public WorkflowCommandHandler(
            IMediator mediator,
            ILogger<BaseCommandHandler<WorkflowCommand, WorkflowCommandResultApp>> logger,
            IEventLogger eventLogger,
            IHttpContextAccessor httpContextAccessor,
            IWorkflowEngine workflowEngine,
            IWorkflowRepository workflowRepository,
            IWorkflowRecoveryService recoveryService)
            : base(mediator, logger, eventLogger, httpContextAccessor)
        {
            _workflowEngine = workflowEngine;
            _workflowRepository = workflowRepository;
            _recoveryService = recoveryService;
        }

        protected override async Task<ApiResponse<WorkflowCommandResultApp>> HandleCommand(
            WorkflowCommand request, CancellationToken cancellationToken)
        {
            var stopwatch = Stopwatch.StartNew();
            Dictionary<string, object> diagnosticInfo = new();

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
                    ModuleProperties = request.PropertiesData,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = request.UserId.ToString()
                };

                // Record diagnostic information
                diagnosticInfo["workflowId"] = workflow.Id;
                diagnosticInfo["workflowName"] = workflow.Name;
                diagnosticInfo["moduleType"] = moduleData.ModuleType;

                // 3. Create a checkpoint before starting the workflow
                Dictionary<string, object> initialCheckpoint = new()
                {
                    ["requestData"] = request.PropertiesData,
                    ["actionId"] = request.ActionId
                };

                // 4. Start workflow execution
                var workflowInstance = await _workflowEngine.StartWorkflowAsync(
                    workflow.Id,
                    moduleData,
                    initialCheckpoint); // Pass checkpoint data as initial variables

                // Create initial checkpoint
                await _recoveryService.CreateCheckpointAsync(workflowInstance.Id, initialCheckpoint);
                
                // Record instance ID in diagnostics
                diagnosticInfo["instanceId"] = workflowInstance.Id;
                diagnosticInfo["startTime"] = workflowInstance.StartedAt;
                
                // 5. Check if workflow completed synchronously or needs to run in background
                if (workflowInstance.Status == WorkflowStatus.Completed)
                {
                    // Workflow completed - retrieve result from output variables
                    stopwatch.Stop();
                    diagnosticInfo["executionTime"] = stopwatch.Elapsed;
                    
                    return ApiResponse<WorkflowCommandResultApp>.Success(new WorkflowCommandResultApp
                    {
                        Success = true,
                        Result = ExtractPrimaryResult(workflowInstance.Variables),
                        OutputVariables = workflowInstance.Variables,
                        Message = "Workflow completed successfully",
                        WorkflowInstanceId = workflowInstance.Id,
                        ExecutionTime = stopwatch.Elapsed,
                        DiagnosticInfo = diagnosticInfo
                    });
                }

                // 6. Workflow is running in the background - return reference to track it
                return ApiResponse<WorkflowCommandResultApp>.Success(new WorkflowCommandResultApp
                {
                    Success = true,
                    Result = new { WorkflowId = workflowInstance.Id },
                    Message = "Workflow started successfully and is running in the background",
                    WorkflowInstanceId = workflowInstance.Id,
                    DiagnosticInfo = diagnosticInfo
                });
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "Error executing workflow for action {ActionId}", request.ActionId);
                
                // Add error information to diagnostics
                diagnosticInfo["error"] = ex.Message;
                diagnosticInfo["errorType"] = ex.GetType().Name;
                diagnosticInfo["stackTrace"] = ex.StackTrace;
                
                return ApiResponse<WorkflowCommandResultApp>.Fail(
                    "WORKFLOW_EXECUTION_ERROR", 
                    new WorkflowCommandResultApp
                    {
                        Success = false,
                        Message = $"Error executing workflow: {ex.Message}",
                        ErrorCode = "WORKFLOW_EXECUTION_ERROR",
                        Exception = ex,
                        DiagnosticInfo = diagnosticInfo,
                        ExecutionTime = stopwatch.Elapsed,
                        CanRetry = true
                    }
                );
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
