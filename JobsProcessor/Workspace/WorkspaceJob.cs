using Events.Modules.Workspace;

namespace JobsProcessor.Workspace
{
    public class WorkspaceJob : IWorkspaceJob
    {
        public void ProcessWorkspaceCreatedEvent(WorkspaceCreatedEvent notification)
        {
            Console.WriteLine($"Title is {notification.Title}");
        }
    }
}
