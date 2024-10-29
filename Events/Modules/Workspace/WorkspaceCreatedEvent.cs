namespace Events.Modules.Workspace
{
    public class WorkspaceCreatedEvent : BaseEvent, ICreatedEvent
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
    }
}
