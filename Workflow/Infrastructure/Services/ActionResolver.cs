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
        private  IDictionary<string, Type> _actionTypes;

        public ActionResolver(
            IServiceProvider serviceProvider,
            ILogger<ActionResolver> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
          
        }

        public IWorkflowAction ResolveAction(string actionType, IServiceScope s)
        {
            _actionTypes = _serviceProvider.GetServices<IWorkflowAction>().ToDictionary(c => c.GetType().Name, c => c.GetType());
            try
            {
                if (string.IsNullOrEmpty(actionType))
                {
                    throw new WorkflowValidationException("Action type cannot be empty", "INVALID_ACTION_TYPE");
                }

                if (!_actionTypes.TryGetValue(actionType, out var implementationType))
                {
                    throw new WorkflowValidationException($"No action implementation found for type: {actionType}", "ACTION_NOT_FOUND");
                }

                
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
            if (string.IsNullOrEmpty(actionType))
                throw new ArgumentNullException(nameof(actionType));

            _actionTypes[actionType] = typeof(TAction);
            _logger.LogInformation("Registered action type: {ActionType} -> {ImplementationType}",
                actionType, typeof(TAction).Name);
        }

        
    }
}
