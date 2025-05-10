using AppWorkflow.Core.Interfaces.Services;
using AppWorkflow.Core.Models;
using AppWorkflow.Infrastructure.Data.Configurations;
using Microsoft.Extensions.Logging;

namespace AppWorkflow.Infrastructure.Triggers;

/// <summary>
/// Handler for manually triggered workflows 
/// </summary>
public class ManualTriggerHandler : IWorkflowTriggerHandler
{
    private readonly ILogger<ManualTriggerHandler> _logger;
    private readonly IWorkflowEngine _engine;
    
    public string TriggerType => "Manual";
    
    public ManualTriggerHandler(
        ILogger<ManualTriggerHandler> logger,
        IWorkflowEngine engine)
    {
        _logger = logger;
        _engine = engine;
    }
    
    public bool CanHandle(string triggerType)
    {
        return string.Equals(triggerType, TriggerType, StringComparison.OrdinalIgnoreCase);
    }
    
    public async Task<bool> HandleTriggerAsync(TriggerContext context)
    {
        try
        {
            _logger.LogInformation("Processing manual trigger for workflow {WorkflowId}", context.WorkflowId);
            // Create a WorkflowModuleData from the parameters
            var moduleData = new WorkflowModuleData
            {
                ModuleType = context.ModuleType,
                ModuleProperties = context.Data.ModuleProperties,
            };

            // Start the workflow instance with the provided parameters
            await _engine.StartWorkflowAsync(context.WorkflowId, moduleData, context.Parameters);
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling manual trigger for workflow {WorkflowId}", context.WorkflowId);
            return false;
        }
    }
    
    public Task RegisterTriggerAsync(TriggerConfiguration configuration)
    {
        // Manual triggers don't require registration
        _logger.LogInformation("Manual trigger available for workflow {WorkflowId}", configuration.WorkflowId);
        return Task.CompletedTask;
    }
    
    public Task UnregisterTriggerAsync(Guid workflowId)
    {
        // Nothing to unregister for manual triggers
        return Task.CompletedTask;
    }
}
