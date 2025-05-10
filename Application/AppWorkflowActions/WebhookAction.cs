using AppWorkflow.Core.Interfaces.Services;
using AppWorkflow.Infrastructure.Services.Actions;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Threading.Tasks;

namespace Application.AppWorkflowActions;

public class WebhookAction : WorkflowActionBase
{
    public WebhookAction(ILogger<WebhookAction> logger, IServiceProvider serviceProvider) : base(logger, serviceProvider) { }

    public override async Task<ActionResult> ExecuteAsync(ActionContext context)
    {
        // TODO: Implement webhook call logic using config
        return await CreateSuccessResult("Webhook called");
    }

    public override Task<ValidationResult> ValidateConfigurationAsync(JsonDocument config)
        => Task.FromResult(new ValidationResult { IsValid = true });

    public override Task<object> GetConfigurationSchemaAsync()
        => Task.FromResult<object>(new { url = "string", method = "string", payload = "object" });
}

public class WebhookActionConfiguration
{
    public string Url { get; set; }
    public string Method { get; set; } = "POST";
    public object Payload { get; set; }
}
