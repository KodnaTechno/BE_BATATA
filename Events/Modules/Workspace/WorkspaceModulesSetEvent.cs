
namespace Events.Modules.Workspace
{
    public class WorkspaceModulesSetEvent : BaseEvent, ICreatedEvent
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
    }
}
