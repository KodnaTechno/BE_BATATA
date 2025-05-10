using AppWorkflow.Core.Interfaces.Services;
using AppWorkflow.Core.Models;
using AppWorkflow.Infrastructure.Data.Configurations;
using Microsoft.Extensions.Logging;

namespace AppWorkflow.Infrastructure.Triggers;

/// <summary>
/// Handler for scheduled/timed workflow triggers
/// </summary>
public class ScheduledTriggerHandler : IWorkflowTriggerHandler
{
    private readonly ILogger<ScheduledTriggerHandler> _logger;
    private readonly IWorkflowEngine _engine;
    
    public string TriggerType => "Scheduled";
    
    public ScheduledTriggerHandler(
        ILogger<ScheduledTriggerHandler> logger,
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
            _logger.LogInformation("Processing scheduled trigger for workflow {WorkflowId}", context.WorkflowId);
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
            _logger.LogError(ex, "Error handling scheduled trigger for workflow {WorkflowId}", context.WorkflowId);
            return false;
        }
    }
    
    public Task RegisterTriggerAsync(TriggerConfiguration configuration)
    {
        // Registration is handled by the ScheduledTriggerBackgroundService
        // which will scan for scheduled triggers and execute them
        _logger.LogInformation("Registered scheduled trigger for workflow {WorkflowId} with schedule {Schedule}", 
            configuration.WorkflowId, configuration.Schedule);
        
        return Task.CompletedTask;
    }
    
    public Task UnregisterTriggerAsync(Guid workflowId)
    {
        _logger.LogInformation("Unregistered scheduled triggers for workflow {WorkflowId}", workflowId);
        return Task.CompletedTask;
    }
}
