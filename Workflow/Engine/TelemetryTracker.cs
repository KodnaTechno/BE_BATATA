namespace AppWorkflow.Engine;
using AppWorkflow.Core.Domain.Data;
using Microsoft.Extensions.Logging;

public interface ITelemetryTracker
    {
        Task TrackWorkflowStarted(WorkflowData instance);
        //Task TrackWorkflowCompleted(WorkflowData instance);
        //Task TrackStepStatusChanged(WorkflowData instance, WorkflowStepData step);
        //Task TrackStepFailed(WorkflowData instance, WorkflowStep step, Exception exception);
    }
    public class TelemetryTracker : ITelemetryTracker
    {
        private readonly ILogger<TelemetryTracker> _logger;
        //private readonly ITelemetryClient _telemetryClient;

        public async Task TrackWorkflowStarted(WorkflowData instance)
        {
            var properties = new Dictionary<string, string>
            {
                ["workflowId"] = instance.WorkflowId.ToString(),
                ["instanceId"] = instance.Id.ToString(),
                //["moduleType"] = instance.ModuleData.GetProperty("type").GetString()
            };

          //  _telemetryClient.TrackEvent("WorkflowStarted", properties);
        }

        // Implementation of other interface methods...
    }