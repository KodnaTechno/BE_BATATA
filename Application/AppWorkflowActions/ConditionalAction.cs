using AppWorkflow.Core.Interfaces.Services;
using AppWorkflow.Infrastructure.Services.Actions;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Threading.Tasks;

namespace Application.AppWorkflowActions;

public class ConditionalAction : WorkflowActionBase
{
    public ConditionalAction(ILogger<ConditionalAction> logger, IServiceProvider serviceProvider) : base(logger, serviceProvider) { }

    public override async Task<ActionResult> ExecuteAsync(ActionContext context)
    {
        // TODO: Implement conditional logic using config
        return await CreateSuccessResult("Condition evaluated");
    }

    public override Task<ValidationResult> ValidateConfigurationAsync(JsonDocument config)
        => Task.FromResult(new ValidationResult { IsValid = true });

    public override Task<object> GetConfigurationSchemaAsync()
        => Task.FromResult<object>(new { expression = "string" });
}

public class ConditionalActionConfiguration
{
    public string Expression { get; set; }
}
