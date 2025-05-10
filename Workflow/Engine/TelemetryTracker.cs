namespace AppWorkflow.Engine;
using AppWorkflow.Core.Domain.Data;
using Microsoft.Extensions.Logging;

public interface ITelemetryTracker
{
    Task TrackWorkflowStarted(WorkflowData instance);
    Task TrackWorkflowCompleted(WorkflowData instance, TimeSpan duration);
    Task TrackWorkflowError(WorkflowData instance, Exception exception);
    Task TrackStepStarted(WorkflowData instance, Guid stepId);
    Task TrackStepCompleted(WorkflowData instance, Guid stepId, TimeSpan duration, bool success);
    Task TrackStepError(WorkflowData instance, Guid stepId, Exception exception);
}

public class TelemetryTracker : ITelemetryTracker
{
    private readonly ILogger<TelemetryTracker> _logger;
    //private readonly ITelemetryClient _telemetryClient;

    public TelemetryTracker(ILogger<TelemetryTracker> logger)
    {
        _logger = logger;
    }

    public async Task TrackWorkflowStarted(WorkflowData instance)
    {
        _logger.LogInformation("Telemetry: Workflow started {WorkflowId} Instance {InstanceId}", instance.WorkflowId, instance.Id);
        await Task.CompletedTask;
    }

    public async Task TrackWorkflowCompleted(WorkflowData instance, TimeSpan duration)
    {
        _logger.LogInformation("Telemetry: Workflow completed {WorkflowId} Instance {InstanceId} Duration {Duration}", instance.WorkflowId, instance.Id, duration);
        await Task.CompletedTask;
    }

    public async Task TrackWorkflowError(WorkflowData instance, Exception exception)
    {
        _logger.LogError(exception, "Telemetry: Workflow error {WorkflowId} Instance {InstanceId}", instance.WorkflowId, instance.Id);
        await Task.CompletedTask;
    }

    public async Task TrackStepStarted(WorkflowData instance, Guid stepId)
    {
        _logger.LogInformation("Telemetry: Step started {WorkflowId} Instance {InstanceId} Step {StepId}", instance.WorkflowId, instance.Id, stepId);
        await Task.CompletedTask;
    }

    public async Task TrackStepCompleted(WorkflowData instance, Guid stepId, TimeSpan duration, bool success)
    {
        _logger.LogInformation("Telemetry: Step completed {WorkflowId} Instance {InstanceId} Step {StepId} Duration {Duration} Success {Success}", instance.WorkflowId, instance.Id, stepId, duration, success);
        await Task.CompletedTask;
    }

    public async Task TrackStepError(WorkflowData instance, Guid stepId, Exception exception)
    {
        _logger.LogError(exception, "Telemetry: Step error {WorkflowId} Instance {InstanceId} Step {StepId}", instance.WorkflowId, instance.Id, stepId);
        await Task.CompletedTask;
    }
}