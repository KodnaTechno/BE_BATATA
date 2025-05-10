
using AppWorkflow.Core.Interfaces.Services;
using AppWorkflow.Engine;
using AppWorkflow.Infrastructure.Data.Context;
using AppWorkflow.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppWorkflow.Infrastructure.Services.Advance
{
    public class ParallelExecutionManager
    {
        private readonly IWorkflowEngine _workflowEngine;
        private readonly IWorkflowStateManager _stateManager;
        private readonly ILogger<ParallelExecutionManager> _logger;
        private readonly IDistributedLockManager _lockManager;

        public ParallelExecutionManager(
            IWorkflowEngine workflowEngine,
            IWorkflowStateManager stateManager,
            IDistributedLockManager lockManager,
            ILogger<ParallelExecutionManager> logger)
        {
            _workflowEngine = workflowEngine;
            _stateManager = stateManager;
            _lockManager = lockManager;
            _logger = logger;
        }

        public async Task<IEnumerable<StepExecutionResult>> ExecuteParallelStepsAsync(
            WorkflowExecutionContext context,
            IEnumerable<WorkflowStep> parallelSteps)
        {
            var tasks = new List<Task<StepExecutionResult>>();
            var results = new List<StepExecutionResult>();
            var lockKeys = new List<string>();

            try
            {
                // Acquire locks for all steps
                foreach (var step in parallelSteps)
                {
                    var lockKey = $"step-execution-{context.InstanceId}-{step.Id}";
                    await _lockManager.AcquireLockAsync(lockKey, TimeSpan.FromMinutes(5));
                    lockKeys.Add(lockKey);
                }

                // Execute steps in parallel
                foreach (var step in parallelSteps)
                {
                    var task = _workflowEngine.ExecuteStepAsync(context.InstanceId, step.Id);
                    tasks.Add(task);
                }

                // Wait for all steps to complete or timeout
                var completedTasks = await Task.WhenAll(tasks);
                results.AddRange(completedTasks);

                // Update workflow state with parallel execution results
                await UpdateWorkflowStateWithParallelResults(context, results);

                return results;
            }
            finally
            {
                // Release all locks
                foreach (var lockKey in lockKeys)
                {
                    await _lockManager.ReleaseLockAsync(lockKey);
                }
            }
        }

        private async Task UpdateWorkflowStateWithParallelResults(
            WorkflowExecutionContext context,
            IEnumerable<StepExecutionResult> results)
        {
            // Merge variables from all parallel executions
            foreach (var result in results)
            {
                if (result.OutputVariables != null)
                {
                    foreach (var (key, value) in result.OutputVariables)
                    {
                        context.Variables[$"parallel_{key}"] = value;
                    }
                }
            }

            await _stateManager.SaveStateAsync(context);
        }
    }
}
