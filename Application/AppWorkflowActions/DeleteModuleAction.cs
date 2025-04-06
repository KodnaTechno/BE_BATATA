using AppWorkflow.Core.Interfaces.Services;
using AppWorkflow.Infrastructure.Services.Actions;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Application.AppWorkflowActions
{
    public class DeleteModuleAction : WorkflowActionBase
    {
        public DeleteModuleAction(ILogger<DeleteModuleAction> logger, IServiceProvider serviceProvider) : base(logger, serviceProvider)
        {
        }

        public override Task<ActionResult> ExecuteAsync(ActionContext context)
            => throw new NotImplementedException();

        public override Task<ValidationResult> ValidateConfigurationAsync(JsonDocument config)
            => throw new NotImplementedException();

        public override Task<object> GetConfigurationSchemaAsync()
            => throw new NotImplementedException();
    }
}
