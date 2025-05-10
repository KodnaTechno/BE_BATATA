using AppWorkflow.Infrastructure.Data.Configurations;
using Microsoft.Extensions.Logging;

namespace AppWorkflow.Infrastructure.Triggers;

public class ManualTrigger : IWorkflowTrigger
{
    private readonly ILogger<ManualTrigger> _logger;
    public string TriggerType => "Manual";

    public ManualTrigger(ILogger<ManualTrigger> logger)
    {
        _logger = logger;
    }

    public Task<bool> EvaluateAsync(TriggerContext context)
    {
        // Manual triggers always evaluate to true when invoked
        return Task.FromResult(true);
    }

    public Task SubscribeAsync(TriggerConfiguration config)
    {
        // No-op for manual triggers
        return Task.CompletedTask;
    }

    public Task UnsubscribeAsync(Guid workflowId)
    {
        // No-op for manual triggers
        return Task.CompletedTask;
    }
}
