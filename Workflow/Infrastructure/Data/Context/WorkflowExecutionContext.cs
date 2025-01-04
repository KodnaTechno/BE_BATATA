namespace AppWorkflow.Infrastructure.Data.Context;

using System.Text;
using AppWorkflow.Common.Enums;
using AppWorkflow.Core.Domain.Schema;
using AppWorkflow.Core.Models;
using Microsoft.Extensions.DependencyInjection;

public class WorkflowExecutionContext
{
    public Guid WorkflowId { get; set; }
    public Guid InstanceId { get; set; }
    public Guid CurrentStepId { get; set; }
    public WorkflowModuleData ModuleData { get; set; }
    public Dictionary<string, object> Variables { get; set; }
    public WorkflowStatus Status { get; set; }
    public CancellationToken CancellationToken { get; set; }
    public IServiceProvider ServiceProvider { get; set; }
    public Dictionary<string, object> StepData { get; set; }
    public RetryPolicy RetryPolicy { get; set; }
    public string CurrentUser { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? LastUpdatedTime { get; set; }
    public List<string> ExecutionPath { get; set; }
    public Dictionary<string, string> Metadata { get; set; }

    public WorkflowExecutionContext(Guid workflowId, Guid instanceId, IServiceProvider serviceProvider)
    {
        WorkflowId = workflowId;
        InstanceId = instanceId;
        ServiceProvider = serviceProvider;
        Variables = new Dictionary<string, object>();
        StepData = new Dictionary<string, object>();
        ExecutionPath = new List<string>();
        Metadata = new Dictionary<string, string>();
        StartTime = DateTime.UtcNow;
        Status = WorkflowStatus.Active;
    }

    public T GetService<T>() where T : notnull
    {
        return ServiceProvider.GetRequiredService<T>();
    }

    public object GetVariable(string key)
    {
        return Variables.TryGetValue(key, out var value) ? value : null;
    }

    public T GetVariable<T>(string key, T defaultValue = default)
    {
        if (Variables.TryGetValue(key, out var value) && value is T typedValue)
        {
            return typedValue;
        }
        return defaultValue;
    }

    public void SetVariable(string key, object value)
    {
        Variables[key] = value;
        LastUpdatedTime = DateTime.UtcNow;
    }

    public void SetStepData(string key, object value)
    {
        StepData[key] = value;
        LastUpdatedTime = DateTime.UtcNow;
    }

    public T GetStepData<T>(string key, T defaultValue = default)
    {
        if (StepData.TryGetValue(key, out var value) && value is T typedValue)
        {
            return typedValue;
        }
        return defaultValue;
    }

    public void AddToExecutionPath(string stepName)
    {
        ExecutionPath.Add($"{stepName} - {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss.fff}");
    }

    public void UpdateStatus(WorkflowStatus newStatus)
    {
        Status = newStatus;
        LastUpdatedTime = DateTime.UtcNow;
    }

    public void SetMetadata(string key, string value)
    {
        Metadata[key] = value;
        LastUpdatedTime = DateTime.UtcNow;
    }

    public string GetMetadata(string key)
    {
        return Metadata.TryGetValue(key, out var value) ? value : null;
    }

    public bool HasVariable(string key)
    {
        return Variables.ContainsKey(key);
    }

    public void RemoveVariable(string key)
    {
        Variables.Remove(key);
        LastUpdatedTime = DateTime.UtcNow;
    }

    public void ClearVariables()
    {
        Variables.Clear();
        LastUpdatedTime = DateTime.UtcNow;
    }

    public TimeSpan GetExecutionDuration()
    {
        return DateTime.UtcNow - StartTime;
    }

    public Dictionary<string, object> GetExecutionSummary()
    {
        return new Dictionary<string, object>
        {
            ["workflowId"] = WorkflowId,
            ["instanceId"] = InstanceId,
            ["status"] = Status,
            ["startTime"] = StartTime,
            ["lastUpdated"] = LastUpdatedTime,
            ["duration"] = GetExecutionDuration(),
            ["currentStep"] = CurrentStepId,
            ["executionPath"] = ExecutionPath,
            ["variableCount"] = Variables.Count,
            ["metadata"] = Metadata
        };
    }
}