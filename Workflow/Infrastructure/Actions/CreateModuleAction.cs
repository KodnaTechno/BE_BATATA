﻿using AppWorkflow.Core.Interfaces.Services;
using AppWorkflow.Infrastructure.Services.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppWorkflow.Infrastructure.Actions
{
    public class CreateModuleAction : WorkflowActionBase
    {
        public CreateModuleAction(ILogger<CreateModuleAction> logger, IServiceProvider serviceProvider) : base(logger, serviceProvider)
        {
        }

        public override Task<ActionResult> ExecuteAsync(ActionContext context)
        {
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
