using AppWorkflow.Infrastructure.Data.Configurations;
using Microsoft.Extensions.Logging;

namespace AppWorkflow.Infrastructure.Triggers;

public class ExternalTrigger : IWorkflowTrigger
{
    private readonly ILogger<ExternalTrigger> _logger;
    public string TriggerType => "External";

    public ExternalTrigger(ILogger<ExternalTrigger> logger)
    {
        _logger = logger;
    }

    public Task<bool> EvaluateAsync(TriggerContext context)
    {
        // External triggers may have custom logic, for now always true
        return Task.FromResult(true);
    }

    public Task SubscribeAsync(TriggerConfiguration config)
    {
        // Register webhook or external event subscription here
        return Task.CompletedTask;
    }

    public Task UnsubscribeAsync(Guid workflowId)
    {
        // Unregister webhook or external event subscription here
        return Task.CompletedTask;
    }
}
