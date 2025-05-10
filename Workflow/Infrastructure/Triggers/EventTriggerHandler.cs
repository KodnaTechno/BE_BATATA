using AppWorkflow.Core.Interfaces.Services;
using AppWorkflow.Core.Models;
using AppWorkflow.Infrastructure.Data.Configurations;
using Microsoft.Extensions.Logging;

namespace AppWorkflow.Infrastructure.Triggers;

/// <summary>
/// Handler for event-based workflow triggers
/// </summary>
public class EventTriggerHandler : IWorkflowTriggerHandler
{
    private readonly ILogger<EventTriggerHandler> _logger;
    private readonly IWorkflowEngine _engine;
    private readonly IWorkflowEventHandler _eventHandler;
    
    public string TriggerType => "Event";
    
    public EventTriggerHandler(
        ILogger<EventTriggerHandler> logger,
        IWorkflowEngine engine,
        IWorkflowEventHandler eventHandler)
    {
        _logger = logger;
        _engine = engine;
        _eventHandler = eventHandler;
    }
    
    public bool CanHandle(string triggerType)
    {
        return string.Equals(triggerType, TriggerType, StringComparison.OrdinalIgnoreCase);
    }
    
    public async Task<bool> HandleTriggerAsync(TriggerContext context)
    {
        try
        {
            _logger.LogInformation("Processing event trigger for workflow {WorkflowId}", context.WorkflowId);
              // Validate the event against the trigger criteria
            if (context.EventName == null)
            {
                _logger.LogWarning("Event trigger missing event name");
                return false;
            }
            
            // Create a WorkflowModuleData from the parameters
            var moduleData = new WorkflowModuleData 
            {
                ModuleType = context.ModuleType,
                ModuleProperties = context.Data.ModuleProperties,
            };
            
            // Start the workflow instance
            await _engine.StartWorkflowAsync(context.WorkflowId, moduleData, context.Parameters);
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling event trigger for workflow {WorkflowId}", context.WorkflowId);
            return false;
        }
    }
      public Task RegisterTriggerAsync(TriggerConfiguration configuration)
    {
        try
        {
            _logger.LogInformation("Registering event trigger for workflow {WorkflowId} with event {EventName}", 
                configuration.WorkflowId, configuration.EventName);
            
            //// Register to receive events for this trigger
            //_eventHandler.RegisterEventHandler(
            //    configuration.EventName,
            //    async (eventData) => 
            //    {
            //        var context = new TriggerContext
            //        {
            //            WorkflowId = configuration.WorkflowId,
            //            EventName = configuration.EventName,
            //            Parameters = eventData,
            //            TriggerType = TriggerType
            //        };
                    
            //        await HandleTriggerAsync(context);
            //    });
                
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering event trigger for workflow {WorkflowId}", configuration.WorkflowId);
            throw;
        }
    }
    
    public Task UnregisterTriggerAsync(Guid workflowId)
    {
        try
        {
            _logger.LogInformation("Unregistering event triggers for workflow {WorkflowId}", workflowId);
            
            // Unregister from receiving events for this workflow
            //_eventHandler.UnregisterEventHandlers(workflowId);
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unregistering event triggers for workflow {WorkflowId}", workflowId);
            throw;
        }
    }
}
