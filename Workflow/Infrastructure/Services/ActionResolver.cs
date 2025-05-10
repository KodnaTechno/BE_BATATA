using AppWorkflow.Common.Exceptions;
using AppWorkflow.Infrastructure.Services.Actions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppWorkflow.Infrastructure.Services
{
    public interface IActionResolver
    {
        IWorkflowAction ResolveAction(string actionType,IServiceScope scope);
        void RegisterAction<TAction>(string actionType) where TAction : IWorkflowAction;
    }

    public class ActionResolver : IActionResolver
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ActionResolver> _logger;

        public ActionResolver(
            IServiceProvider serviceProvider,
            ILogger<ActionResolver> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
          
        }

        public IWorkflowAction ResolveAction(string actionType, IServiceScope s)
        {
            // Use the static registry for action lookup
            var implementationType = Actions.WorkflowActionRegistry.GetActionType(actionType);
            if (implementationType == null)
            {
                throw new WorkflowValidationException($"No action implementation found for type: {actionType}");
            }
            try
            {
                var action = (IWorkflowAction)ActivatorUtilities.CreateInstance(s.ServiceProvider, implementationType);
                return action;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to resolve action type: {ActionType}", actionType);
                throw new WorkflowEngineException($"Failed to resolve action: {actionType}", ex);
            }
        }

        public void RegisterAction<TAction>(string actionType) where TAction : IWorkflowAction
        {
            Actions.WorkflowActionRegistry.Register<TAction>(actionType);
            _logger.LogInformation("Registered action type: {ActionType} -> {ImplementationType}",
                actionType, typeof(TAction).Name);
        }
    }
}
