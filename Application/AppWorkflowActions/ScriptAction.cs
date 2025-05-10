using AppWorkflow.Core.Interfaces.Services;
using AppWorkflow.Infrastructure.Services.Actions;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Threading.Tasks;

namespace Application.AppWorkflowActions;

public class ScriptAction : WorkflowActionBase
{
    public ScriptAction(ILogger<ScriptAction> logger, IServiceProvider serviceProvider) : base(logger, serviceProvider) { }

    public override async Task<ActionResult> ExecuteAsync(ActionContext context)
    {
        // TODO: Implement script execution logic using config
        return await CreateSuccessResult("Script executed");
    }

    public override Task<ValidationResult> ValidateConfigurationAsync(JsonDocument config)
        => Task.FromResult(new ValidationResult { IsValid = true });

    public override Task<object> GetConfigurationSchemaAsync()
        => Task.FromResult<object>(new { language = "string", code = "string" });
}

public class ScriptActionConfiguration
{
    public string Language { get; set; } // e.g., "CSharp", "PowerShell", "JavaScript"
    public string Code { get; set; }
}
