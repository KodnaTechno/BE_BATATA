namespace Events.Modules.Properties
{
    public class PropertyCreatedEvent : BaseEvent
    {
        public Guid Id { get; set; }
        public Guid? ModuleId { get; set; }
        public Guid? WorkspaceId { get; set; }
        public Guid? WorkspaceModuleId { get; set; }
        public Guid? ApplicationId { get; set; }
    }
}
