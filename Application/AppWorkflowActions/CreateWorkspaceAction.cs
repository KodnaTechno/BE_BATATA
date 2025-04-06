using AppCommon;
using Application.AppWorkflowActions;
using AppWorkflow.Core.Interfaces.Services;
using AppWorkflow.Infrastructure.Services.Actions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Module;
using Module.Domain.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AppWorkflow.Infrastructure.Actions
{
    public class CreateWorkspaceAction : WorkflowActionBase
    {
        private readonly ModuleDbContext moduleDbContext;
        public CreateWorkspaceAction(ILogger<CreateWorkspaceAction> logger, IServiceProvider serviceProvider, ModuleDbContext moduleDbContext) : base(logger, serviceProvider)
        {
            this.moduleDbContext = moduleDbContext;
        }

        public override async Task<ActionResult> ExecuteAsync(ActionContext context)
        {
            // get workspaceId from workflow meta data
            var workSpaceSchemaIdStr = context.WorkflowExecutionContext.Metadata.GetValueOrDefault("WorksapceId");
            if(Guid.TryParse(workSpaceSchemaIdStr, out Guid workSpaceSchemaId))
            {
                var workspace = new WorkspaceData
                {
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = context.CurrentUserId ?? SystemUsers.SystemUserId,
                    UpdatedAt = DateTime.UtcNow,
                    UpdatedBy = context.CurrentUserId ?? SystemUsers.SystemUserId,
                    WorkspaceId = workSpaceSchemaId,
                    
                };
                try
                {
                    moduleDbContext.WorkspaceData.Add(workspace);
                    await moduleDbContext.SaveChangesAsync();
                    return new ActionResult
                    {
                        Success = true,
                    };
                }
                catch (Exception e)
                {

                    throw;
                }
              
            }

            return new ActionResult {
                Exception = new Exception("WorkSpaceId NOT Found Can't Proceed With Creating WorkSpace Instance"),
                Message="WrokSpaceId Property Not Fond ",
            };
         
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
