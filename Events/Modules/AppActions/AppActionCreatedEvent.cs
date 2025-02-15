namespace Events.Modules.AppActions
{
    public class AppActionCreatedEvent : BaseEvent, ICreatedEvent
    {
        public Guid Id { get; set; }
    }
}
