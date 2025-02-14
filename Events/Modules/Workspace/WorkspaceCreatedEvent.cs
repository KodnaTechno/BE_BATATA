using AppCommon.DTOs;

namespace Events.Modules.Workspace
{
    public class WorkspaceCreatedEvent : BaseEvent, ICreatedEvent
    {
        public Guid Id { get; set; }
        public TranslatableValue Title { get; set; }
    }
}
