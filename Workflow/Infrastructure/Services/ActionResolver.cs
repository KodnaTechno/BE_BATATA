using AppWorkflow.Common.Exceptions;
using AppWorkflow.Infrastructure.Actions;
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
        IWorkflowAction ResolveAction(string actionType);
        void RegisterAction<TAction>(string actionType) where TAction : IWorkflowAction;
    }

    public class ActionResolver : IActionResolver
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ActionResolver> _logger;
        private readonly IDictionary<string, Type> _actionTypes;

        public ActionResolver(
            IServiceProvider serviceProvider,
            ILogger<ActionResolver> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _actionTypes = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
            RegisterBuiltInActions();
        }

        public IWorkflowAction ResolveAction(string actionType)
        {
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

                using var scope = _serviceProvider.CreateScope();
                var action = (IWorkflowAction)ActivatorUtilities.CreateInstance(scope.ServiceProvider, implementationType);
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

        private void RegisterBuiltInActions()
        {
            // Register core action types
            //RegisterAction<CreateModuleAction>("CREATE_MODULE");
            RegisterAction<UpdateModuleAction>("UPDATE_MODULE");
            RegisterAction<DeleteModuleAction>("DELETE_MODULE");
            //RegisterAction<SendNotificationAction>("SEND_NOTIFICATION");
            //RegisterAction<ExecuteQueryAction>("EXECUTE_QUERY");
            //RegisterAction<HttpRequestAction>("HTTP_REQUEST");
            //RegisterAction<FileOperationAction>("FILE_OPERATION");
            //RegisterAction<ValidationAction>("VALIDATE_DATA");
            //RegisterAction<TransformDataAction>("TRANSFORM_DATA");
        }
    }
}
