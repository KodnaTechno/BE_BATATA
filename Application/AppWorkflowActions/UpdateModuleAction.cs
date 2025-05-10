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

    public class UpdateModuleAction : WorkflowActionBase
    {
        public UpdateModuleAction(ILogger<UpdateModuleAction> logger, IServiceProvider serviceProvider) : base(logger, serviceProvider)
        {
        }

        public override async Task<ActionResult> ExecuteAsync(ActionContext context)
        {
            var db = context.ServiceProvider.GetRequiredService<ModuleDbContext>();
            var userId = context.CurrentUserId ?? Guid.Empty;
            var moduleId = context.ModuleData?.Id ?? Guid.Empty;
            var moduleData = await db.ModuleData.FindAsync(moduleId);
            if (moduleData == null)
            {
                return new ActionResult { Success = false, Message = $"ModuleData with Id {moduleId} not found" };
            }
            moduleData.UpdatedAt = DateTime.UtcNow;
            moduleData.UpdatedBy = userId;
            // Update properties
            var propertyDtos = context.ModuleData?.ModuleProperties ?? new List<PropertyDataDto>();
            foreach (var propDto in propertyDtos)
            {
                var prop = moduleData.PropertyData.FirstOrDefault(p => p.PropertyId == propDto.PropertyId);
                if (prop != null)
                {
                    prop.StringValue = propDto.Value;
                }
                else
                {
                    moduleData.PropertyData.Add(new PropertyData
                    {
                        PropertyId = propDto.PropertyId,
                        StringValue = propDto.Value
                    });
                }
            }
            try
            {
                await db.SaveChangesAsync();
                return new ActionResult { Success = true, Message = "Module updated successfully" };
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
