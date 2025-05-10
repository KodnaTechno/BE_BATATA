using AppWorkflow.Core.Interfaces.Services;
using AppWorkflow.Infrastructure.Services.Actions;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Threading.Tasks;

namespace Application.AppWorkflowActions;

public class SendEmailAction : WorkflowActionBase
{
    public SendEmailAction(ILogger<SendEmailAction> logger, IServiceProvider serviceProvider) : base(logger, serviceProvider) { }

    public override async Task<ActionResult> ExecuteAsync(ActionContext context)
    {
        // TODO: Implement email sending logic using config
        return await CreateSuccessResult("Email sent");
    }

    public override Task<ValidationResult> ValidateConfigurationAsync(JsonDocument config)
        => Task.FromResult(new ValidationResult { IsValid = true });

    public override Task<object> GetConfigurationSchemaAsync()
        => Task.FromResult<object>(new { to = "string", subject = "string", body = "string" });
}

public class SendEmailActionConfiguration
{
    public string To { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
}
