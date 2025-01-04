using AppWorkflow.Core.Domain.Schema;
using AppWorkflow.Core.Interfaces.Services;
using AppWorkflow.Infrastructure.Repositories.IRepository;
using AppWorkflow.Infrastructure.Triggers;
using Microsoft.Extensions.Logging;

namespace AppWorkflow.Triggers;

public class TriggerManager
    {
        private readonly ILogger<TriggerManager> _logger;
        private readonly IWorkflowEngine _workflowEngine;
        private readonly IWorkflowRepository _workflowRepository;
        private readonly IDictionary<string, IWorkflowTrigger> _triggers;

        public async Task HandleTriggerEventAsync(TriggerContext context)
        {
            try
            {
                // Find workflows that match the trigger
                var matchingWorkflows = await _workflowRepository.GetWorkflowsByTriggerTypeAsync(
                    context.TriggerType,
                    context.ModuleType
                );

                foreach (var workflow in matchingWorkflows)
                {
                    var trigger = _triggers[context.TriggerType];

                    // Evaluate trigger conditions
                    if (await trigger.EvaluateAsync(context))
                    {
                        // Start workflow instance
                        await _workflowEngine.StartWorkflowAsync(
                            workflow.Id,
                            context.Data,
                            new Dictionary<string, object>
                            {
                                ["triggerContext"] = context
                            }
                        );

                        //await _telemetry.TrackTriggerExecuted(workflow.Id, context);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error handling trigger event: {context.TriggerType}");
                throw;
            }
        }

        public async Task RegisterTriggerAsync(Workflow workflow)
        {
            foreach (var triggerConfig in workflow.TriggerConfigs)
            {
                if (_triggers.TryGetValue(triggerConfig.Type, out var trigger))
                {
                    await trigger.SubscribeAsync(triggerConfig);
                }
                else
                {
                    //throw new InvalidTriggerTypeException(triggerConfig.Type);
                }
            }
        }

        public async Task UnregisterTriggerAsync(Guid workflowId)
        {
            foreach (var trigger in _triggers.Values)
            {
                await trigger.UnsubscribeAsync(workflowId);
            }
        }
    }