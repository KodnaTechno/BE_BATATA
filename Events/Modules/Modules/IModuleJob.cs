namespace Events.Modules.Modules
{
    public interface IModuleJob
    {
        void ProcessModuleCreatedEvent(ModuleCreatedEvent @event);
    }
}
