namespace Events.Modules.Workspace
{
    public interface IWorkspaceJob
    {
        void ProcessWorkspaceCreatedEvent(WorkspaceCreatedEvent @event);
        void ProcessWorkspaceModulesAssignedEvent(WorkspaceModulesAssignedEvent notification);
    }
}
