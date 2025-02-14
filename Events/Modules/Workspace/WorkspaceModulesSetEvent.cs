
namespace Events.Modules.Workspace
{
    public class WorkspaceModulesSetEvent : BaseEvent, ICreatedEvent
    {
        public Guid Id { get; set; }
    }
}
