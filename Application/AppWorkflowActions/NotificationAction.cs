using AppWorkflow.Core.Interfaces.Services;
using AppWorkflow.Infrastructure.Services.Actions;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Threading.Tasks;

namespace Application.AppWorkflowActions;

public class NotificationAction : WorkflowActionBase
{
    public NotificationAction(ILogger<NotificationAction> logger, IServiceProvider serviceProvider) : base(logger, serviceProvider) { }

    public override async Task<ActionResult> ExecuteAsync(ActionContext context)
    {
        try
        {
            // TODO: Implement notification logic using config
            return new ActionResult {
                Success = true,
                Message = "Notification sent",
                Command = StepCommandType.Completed
            };
        }
        catch (Exception ex)
        {
            return new ActionResult {
                Success = false,
                Exception = ex,
                Message = $"Failed to send notification: {ex.Message}",
                Command = StepCommandType.Failed
            };
        }
    }

    public override Task<ValidationResult> ValidateConfigurationAsync(JsonDocument config)
        => Task.FromResult(new ValidationResult { IsValid = true });

    public override Task<object> GetConfigurationSchemaAsync()
        => Task.FromResult<object>(new { userId = "string", message = "string" });
}

public class NotificationActionConfiguration
{
    public string UserId { get; set; }
    public string Message { get; set; }
}
