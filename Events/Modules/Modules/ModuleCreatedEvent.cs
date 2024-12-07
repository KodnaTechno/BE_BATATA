namespace Events.Modules.Modules
{
    public class ModuleCreatedEvent : BaseEvent, ICreatedEvent
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
    }
}
