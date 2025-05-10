using AppWorkflow.Core.Interfaces.Services;
using AppWorkflow.Infrastructure.Services.Actions;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Threading.Tasks;

namespace Application.AppWorkflowActions;

public class DelayAction : WorkflowActionBase
{
    public DelayAction(ILogger<DelayAction> logger, IServiceProvider serviceProvider) : base(logger, serviceProvider) { }

    public override async Task<ActionResult> ExecuteAsync(ActionContext context)
    {
        // TODO: Implement delay logic using config
        await Task.Delay(1000); // Example: 1 second
        return await CreateSuccessResult("Delay completed");
    }

    public override Task<ValidationResult> ValidateConfigurationAsync(JsonDocument config)
        => Task.FromResult(new ValidationResult { IsValid = true });

    public override Task<object> GetConfigurationSchemaAsync()
        => Task.FromResult<object>(new { milliseconds = "int" });
}

public class DelayActionConfiguration
{
    public int Milliseconds { get; set; }
}
