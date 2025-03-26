namespace Events.Modules.Properties
{
    public interface IPropertyJob
    {
        void ProcessPropertyCreatedEvent(PropertyCreatedEvent @event);
        void ProcessPropertyUpdatedEvent(PropertyUpdatedEvent @event);
        void ProcessPropertyDeletedEvent(PropertyDeletedEvent @event);
    }
}
