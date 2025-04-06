using AppWorkflow.Common.Exceptions;
using AppWorkflow.Core.Interfaces.Services;
using AppWorkflow.Infrastructure.Services.Actions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace AppWorkflow.Infrastructure.Services.Actions;

public abstract class WorkflowActionBase : IWorkflowAction
{
    protected readonly ILogger _logger;
    protected readonly IServiceProvider _serviceProvider;

    protected WorkflowActionBase(ILogger logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    public abstract Task<ActionResult> ExecuteAsync(ActionContext context);

    public abstract Task<ValidationResult> ValidateConfigurationAsync(JsonDocument config);

    public abstract Task<object> GetConfigurationSchemaAsync();

    protected async Task<T> GetService<T>() where T : notnull
    {
        using var scope = _serviceProvider.CreateScope();
        return scope.ServiceProvider.GetRequiredService<T>();
    }

    protected async Task<ActionResult> CreateSuccessResult(string message = "Action executed successfully", Dictionary<string, object> outputVariables = null)
    {
        return new ActionResult
        {
            Success = true,
            Message = message,
            OutputVariables = outputVariables ?? new Dictionary<string, object>()
        };
    }

    protected async Task<ActionResult> CreateFailureResult(string message, Exception exception = null)
    {
        return new ActionResult
        {
            Success = false,
            Message = message,
            Exception = exception
        };
    }

    protected async Task<ValidationResult> CreateValidationResult(bool isValid, string message = null, List<string> errors = null)
    {
        return new ValidationResult
        {
            IsValid = isValid,
            //Message = message,
            //Errors = errors ?? new List<string>()
        };
    }

    protected void LogActionStart(ActionContext context)
    {
        _logger.LogInformation(
            "Starting action execution for Step {StepId} in Workflow Instance {InstanceId}",
            context.StepId,
            context.WorkflowDataId
        );
    }

    protected void LogActionComplete(ActionContext context, ActionResult result)
    {
        if (result.Success)
        {
            _logger.LogInformation(
                "Successfully completed action for Step {StepId} in Workflow Instance {InstanceId}",
                context.StepId,
                context.WorkflowDataId
            );
        }
        else
        {
            _logger.LogWarning(
                "Action failed for Step {StepId} in Workflow Instance {InstanceId}: {Message}",
                context.StepId,
                context.WorkflowDataId,
                result.Message
            );
        }
    }

    protected void LogActionError(ActionContext context, Exception ex)
    {
        _logger.LogError(
            ex,
            "Error executing action for Step {StepId} in Workflow Instance {InstanceId}",
            context.StepId,
            context.WorkflowDataId
        );
    }

    protected async Task<T> DeserializeConfiguration<T>(JsonDocument config) where T : class
    {
        try
        {
            return config.Deserialize<T>(new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deserializing configuration");
            throw new WorkflowException("Invalid configuration format", ex);
        }
    }

    protected async Task ValidateRequiredProperties(object config, params string[] requiredProperties)
    {
        var errors = new List<string>();
        var properties = config.GetType().GetProperties();

        foreach (var required in requiredProperties)
        {
            var property = properties.FirstOrDefault(p => p.Name.Equals(required, StringComparison.OrdinalIgnoreCase));
            if (property == null)
            {
                errors.Add($"Required property '{required}' is missing");
                continue;
            }

            var value = property.GetValue(config);
            if (value == null)
            {
                errors.Add($"Required property '{required}' cannot be null");
            }
            else if (value is string strValue && string.IsNullOrWhiteSpace(strValue))
            {
                errors.Add($"Required property '{required}' cannot be empty");
            }
        }

        if (errors.Any())
        {
            throw new WorkflowValidationException("Configuration validation failed", errors);
        }
    }

    protected virtual async Task<bool> PreExecuteAsync(ActionContext context)
    {
        // Override in derived classes if needed
        return true;
    }

    protected virtual async Task PostExecuteAsync(ActionContext context, ActionResult result)
    {
        // Override in derived classes if needed
    }

    protected virtual async Task OnErrorAsync(ActionContext context, Exception exception)
    {
        // Override in derived classes if needed
    }

    protected virtual async Task<Dictionary<string, object>> GatherActionMetrics(ActionContext context, ActionResult result, TimeSpan duration)
    {
        return new Dictionary<string, object>
        {
            ["actionType"] = GetType().Name,
            ["duration"] = duration.TotalMilliseconds,
            ["success"] = result.Success,
            ["hasWarnings"] = result.Warnings?.Any() ?? false,
            ["outputVariablesCount"] = result.OutputVariables?.Count ?? 0
        };
    }
}