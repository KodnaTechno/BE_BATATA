using AppWorkflow.Core.Interfaces.Services;
using AppWorkflow.Core.Models;
using AppWorkflow.Infrastructure.Data.Configurations;
using Microsoft.Extensions.Logging;

namespace AppWorkflow.Infrastructure.Triggers;

/// <summary>
/// Handler for API-based workflow triggers
/// </summary>
public class ApiTriggerHandler : IWorkflowTriggerHandler
{
    private readonly ILogger<ApiTriggerHandler> _logger;
    private readonly IWorkflowEngine _engine;
    
    public string TriggerType => "Api";
    
    public ApiTriggerHandler(
        ILogger<ApiTriggerHandler> logger,
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
            _logger.LogInformation("Processing API trigger for workflow {WorkflowId} with route {Route}",
                context.WorkflowId, context.ApiRoute);
                  // Validate the API route if needed
            if (string.IsNullOrEmpty(context.ApiRoute))
            {
                _logger.LogWarning("API trigger missing route information");
            }
            
            // Create a WorkflowModuleData from the parameters
            var moduleData = new WorkflowModuleData 
            {
                ModuleType = context.ModuleType,
                ModuleProperties = context.Data?.ModuleProperties,
            };
            
            // Start the workflow instance with the provided parameters
            await _engine.StartWorkflowAsync(context.WorkflowId, moduleData, context.Parameters);
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling API trigger for workflow {WorkflowId}", context.WorkflowId);
            return false;
        }
    }
    
    public Task RegisterTriggerAsync(TriggerConfiguration configuration)
    {
        // API routes are handled by the API controllers
        _logger.LogInformation("Registered API trigger for workflow {WorkflowId} with route {Route}", 
            configuration.WorkflowId, configuration.ApiRoute);
            
        return Task.CompletedTask;
    }
    
    public Task UnregisterTriggerAsync(Guid workflowId)
    {
        _logger.LogInformation("Unregistered API trigger for workflow {WorkflowId}", workflowId);
        return Task.CompletedTask;
    }
}
