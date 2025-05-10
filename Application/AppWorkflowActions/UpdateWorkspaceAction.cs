using AppWorkflow.Infrastructure.Services.Actions;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Threading.Tasks;

namespace AppWorkflow.Infrastructure.Actions
{
    public class UpdateWorkspaceAction : WorkflowActionBase
    {
        public UpdateWorkspaceAction(ILogger<UpdateWorkspaceAction> logger, IServiceProvider serviceProvider)
            : base(logger, serviceProvider) { }

        public override Task<ActionResult> ExecuteAsync(ActionContext context)
        {
            // TODO: Implement update logic for workspace using context.ModuleData/Properties
            throw new NotImplementedException();
        }

        public override Task<object> GetConfigurationSchemaAsync()
        {
            throw new NotImplementedException();
        }

        public override Task<ValidationResult> ValidateConfigurationAsync(JsonDocument config)
        {
            throw new NotImplementedException();
        }
    }
}
