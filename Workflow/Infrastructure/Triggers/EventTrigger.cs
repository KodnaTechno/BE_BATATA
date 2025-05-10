using AppWorkflow.Infrastructure.Data.Configurations;
using Microsoft.Extensions.Logging;

namespace AppWorkflow.Infrastructure.Triggers;

public class EventTrigger : IWorkflowTrigger
{
    private readonly ILogger<EventTrigger> _logger;
    public string TriggerType => "Event";

    public EventTrigger(ILogger<EventTrigger> logger)
    {
        _logger = logger;
    }

    public Task<bool> EvaluateAsync(TriggerContext context)
    {
        // Event triggers may have custom logic, for now always true
        return Task.FromResult(true);
    }

    public Task SubscribeAsync(TriggerConfiguration config)
    {
        // Subscribe to internal app events here
        return Task.CompletedTask;
    }

    public Task UnsubscribeAsync(Guid workflowId)
    {
        // Unsubscribe from internal app events here
        return Task.CompletedTask;
    }
}
