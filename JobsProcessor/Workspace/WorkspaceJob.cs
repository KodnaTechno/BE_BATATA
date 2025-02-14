using Events.Modules.Workspace;
using Module.Service.DefaultSetupService;

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
            _defaultWorkspaceSetupService.AddDefaultActions(notification.Id, notification.UserId);
            _defaultWorkspaceSetupService.AddDefaultProperties(notification.Id, notification.UserId);

        }
    }
}
