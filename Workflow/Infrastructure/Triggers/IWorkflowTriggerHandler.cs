using AppWorkflow.Infrastructure.Data.Configurations;

namespace AppWorkflow.Infrastructure.Triggers;

/// <summary>
/// Interface defining the contract for workflow trigger handlers.
/// Trigger handlers are responsible for processing triggers from different sources.
/// </summary>
public interface IWorkflowTriggerHandler
{
    /// <summary>
    /// Gets the type of trigger this handler processes
    /// </summary>
    string TriggerType { get; }
    
    /// <summary>
    /// Determines if this handler can handle the specified trigger type
    /// </summary>
    /// <param name="triggerType">Type of trigger</param>
    bool CanHandle(string triggerType);
    
    /// <summary>
    /// Processes an incoming trigger event
    /// </summary>
    /// <param name="context">The context containing trigger information</param>
    Task<bool> HandleTriggerAsync(TriggerContext context);
    
    /// <summary>
    /// Registers a new trigger configuration to listen for events
    /// </summary>
    /// <param name="configuration">The trigger configuration</param>
    Task RegisterTriggerAsync(TriggerConfiguration configuration);
    
    /// <summary>
    /// Unregisters a trigger configuration
    /// </summary>
    /// <param name="workflowId">ID of the workflow whose trigger should be unregistered</param>
    Task UnregisterTriggerAsync(Guid workflowId);
}
