namespace Events.Modules.Workspace
{
    public interface IWorkspaceJob
    {
        void ProcessWorkspaceCreatedEvent(WorkspaceCreatedEvent notification);
    }


    public class WorkspaceJob : IWorkspaceJob
    {
        public void ProcessWorkspaceCreatedEvent(WorkspaceCreatedEvent notification)
        {
            Console.WriteLine($"Title is {notification.Title}");
        }
    }
}
