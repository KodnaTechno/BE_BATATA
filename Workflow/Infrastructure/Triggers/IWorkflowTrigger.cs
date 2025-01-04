using AppWorkflow.Infrastructure.Data.Configurations;

namespace AppWorkflow.Infrastructure.Triggers;

public interface IWorkflowTrigger
    {
        string TriggerType { get; }
        Task<bool> EvaluateAsync(TriggerContext context);
        Task SubscribeAsync(TriggerConfiguration config);
        Task UnsubscribeAsync(Guid workflowId);
    }