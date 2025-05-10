using AppWorkflow.Core.Interfaces.Services;
using AppWorkflow.Infrastructure.Services.Actions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Module;
using Module.Domain.Data;
using AppCommon.DTOs.Modules;
using Microsoft.Extensions.DependencyInjection;

namespace Application.AppWorkflowActions
{
    public class CreateModuleAction : WorkflowActionBase
    {
        public CreateModuleAction(ILogger<CreateModuleAction> logger, IServiceProvider serviceProvider) : base(logger, serviceProvider)
        {
        }

        public override async Task<ActionResult> ExecuteAsync(ActionContext context)
        {
            var db = context.ServiceProvider.GetRequiredService<ModuleDbContext>();
            var userId = context.CurrentUserId ?? Guid.Empty;
            var moduleData = new ModuleData
            {
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userId,
                UpdatedAt = DateTime.UtcNow,
                UpdatedBy = userId,
                ModuleId = context.ModuleData?.Id ?? Guid.Empty,
                PropertyData = context.ModuleData?.ModuleProperties?.Select(p => new PropertyData
                {
                    PropertyId = p.PropertyId,
                    StringValue = p.Value,
                    // Map other value types as needed
                }).ToList() ?? new List<PropertyData>()
            };
            try
            {
                db.ModuleData.Add(moduleData);
                await db.SaveChangesAsync();
                return new ActionResult { Success = true, Message = "Module created successfully" };
            }
            catch (Exception ex)
            {
                return new ActionResult { Success = false, Exception = ex, Message = ex.Message };
            }
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
