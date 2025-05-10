using AppWorkflow.Core.Interfaces.Services;
using System.Text.Json;

namespace AppWorkflow.Infrastructure.Services.Actions;

public interface IWorkflowAction
{
    Task<ActionResult> ExecuteAsync(ActionContext context);
    Task<ValidationResult> ValidateConfigurationAsync(JsonDocument config);
    Task<object> GetConfigurationSchemaAsync();
}

public static class WorkflowActionRegistry
{
    private static readonly Dictionary<string, Type> _actions = new();

    public static void Register<TAction>(string actionType) where TAction : IWorkflowAction
    {
        _actions[actionType] = typeof(TAction);
    }

    public static Type? GetActionType(string actionType)
    {
        return _actions.TryGetValue(actionType, out var type) ? type : null; // null is valid if not found
    }

    public static IEnumerable<string> GetRegisteredActionTypes() => _actions.Keys;
}