using Application.Services.DefaultSetupService;
using Events.Modules.Workspace;

namespace JobsProcessor.Workspace
{
    public class WorkspaceJob : IWorkspaceJob
    {
        private readonly IDefaultWorkspaceSetupService _defaultWorkspaceSetupService;
        public WorkspaceJob(IDefaultWorkspaceSetupService defaultWorkspaceSetupService)
        {
            _defaultWorkspaceSetupService = defaultWorkspaceSetupService;
        }

        public void ProcessWorkspaceCreatedEvent(WorkspaceCreatedEvent notification)
        {
            var addedActions=_defaultWorkspaceSetupService.AddDefaultActions(notification.Id, notification.UserId);
            _defaultWorkspaceSetupService.AddDefaultWorkflows(addedActions, notification.UserId);
            _defaultWorkspaceSetupService.AddDefaultProperties(notification.Id, notification.UserId);

        }

        public void ProcessWorkspaceModulesAssignedEvent(WorkspaceModulesAssignedEvent notification)
        {
            _defaultWorkspaceSetupService
                .AddDefaultActionsForWorkspaceModules(notification.WorkspaceId, notification.UserId);
        }
    }
}
