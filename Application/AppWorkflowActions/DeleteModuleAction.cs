using AppWorkflow.Core.Interfaces.Services;
using AppWorkflow.Infrastructure.Services.Actions;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Module;
using Module.Domain.Data;
using Microsoft.Extensions.DependencyInjection;

namespace Application.AppWorkflowActions
{
    public class DeleteModuleAction : WorkflowActionBase
    {
        public DeleteModuleAction(ILogger<DeleteModuleAction> logger, IServiceProvider serviceProvider) : base(logger, serviceProvider)
        {
        }

        public override async Task<ActionResult> ExecuteAsync(ActionContext context)
        {
            var db = context.ServiceProvider.GetRequiredService<ModuleDbContext>();
            var moduleId = context.ModuleData?.Id ?? Guid.Empty;
            var moduleData = await db.ModuleData.FindAsync(moduleId);
            if (moduleData == null)
            {
                return new ActionResult { Success = false, Message = $"ModuleData with Id {moduleId} not found" };
            }
            db.ModuleData.Remove(moduleData);
            try
            {
                await db.SaveChangesAsync();
                return new ActionResult { Success = true, Message = "Module deleted successfully" };
            }
            catch (Exception ex)
            {
                return new ActionResult { Success = false, Exception = ex, Message = ex.Message };
            }
        }

        public override Task<ValidationResult> ValidateConfigurationAsync(JsonDocument config)
            => throw new NotImplementedException();

        public override Task<object> GetConfigurationSchemaAsync()
            => throw new NotImplementedException();
    }
}
