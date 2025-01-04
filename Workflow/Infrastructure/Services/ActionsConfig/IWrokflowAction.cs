using AppWorkflow.Core.Interfaces.Services;
using System.Text.Json;

namespace AppWorkflow.Infrastructure.Services.Actions;

public interface IWorkflowAction
    {
        Task<ActionResult> ExecuteAsync(ActionContext context);
        Task<ValidationResult> ValidateConfigurationAsync(JsonDocument config);
        Task<object> GetConfigurationSchemaAsync();
    }