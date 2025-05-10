using AppWorkflow.Infrastructure.Data.Configurations;
using Microsoft.Extensions.Logging;

namespace AppWorkflow.Infrastructure.Triggers;

/// <summary>
/// Manager class responsible for routing triggers to appropriate handlers
/// </summary>
public class TriggerManager
{
    private readonly ILogger<TriggerManager> _logger;
    private readonly IEnumerable<IWorkflowTriggerHandler> _handlers;
    
    public TriggerManager(
        ILogger<TriggerManager> logger,
        IEnumerable<IWorkflowTriggerHandler> handlers)
    {
        _logger = logger;
        _handlers = handlers;
    }
    
    /// <summary>
    /// Processes a trigger using the appropriate handler
    /// </summary>
    public async Task<bool> ProcessTriggerAsync(TriggerContext context)
    {
        _logger.LogInformation("Processing trigger of type {TriggerType} for workflow {WorkflowId}",
            context.TriggerType, context.WorkflowId);
            
        var handler = _handlers.FirstOrDefault(h => h.CanHandle(context.TriggerType));
        
        if (handler == null)
        {
            _logger.LogWarning("No handler found for trigger type {TriggerType}", context.TriggerType);
            return false;
        }
        
        return await handler.HandleTriggerAsync(context);
    }
    
    /// <summary>
    /// Registers a trigger configuration with the appropriate handler
    /// </summary>
    public async Task RegisterTriggerAsync(TriggerConfiguration configuration)
    {
        _logger.LogInformation("Registering trigger of type {TriggerType} for workflow {WorkflowId}",
            configuration.Type, configuration.WorkflowId);
            
        var handler = _handlers.FirstOrDefault(h => h.CanHandle(configuration.Type));
        
        if (handler == null)
        {
            _logger.LogWarning("No handler found for trigger type {TriggerType}", configuration.Type);
            return;
        }
        
        await handler.RegisterTriggerAsync(configuration);
    }
    
    /// <summary>
    /// Unregisters all triggers for a workflow
    /// </summary>
    public async Task UnregisterTriggersAsync(Guid workflowId)
    {
        _logger.LogInformation("Unregistering all triggers for workflow {WorkflowId}", workflowId);
        
        foreach (var handler in _handlers)
        {
            await handler.UnregisterTriggerAsync(workflowId);
        }
    }
    
    /// <summary>
    /// Legacy method for handling trigger events - routes to ProcessTriggerAsync
    /// </summary>
    public async Task<bool> HandleTriggerEventAsync(TriggerContext context)
    {
        return await ProcessTriggerAsync(context);
    }
}
