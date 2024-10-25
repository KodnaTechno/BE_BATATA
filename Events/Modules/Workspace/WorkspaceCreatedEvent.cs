namespace Events.Modules.Workspace
{
    public class WorkspaceCreatedEvent : BaseEvent
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
    }
}
