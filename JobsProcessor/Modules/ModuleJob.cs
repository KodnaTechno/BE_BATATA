using Events.Modules.Modules;
using Module.Service.DefaultSetupService;

namespace JobsProcessor.Modules
{
    public class ModuleJob : IModuleJob
    {
        private readonly IDefaultModuleSetupService _defaultModuleSetupService;
        public ModuleJob(IDefaultModuleSetupService defaultModuleSetupService)
        {
            _defaultModuleSetupService = defaultModuleSetupService;
        }

        public void ProcessModuleCreatedEvent(ModuleCreatedEvent @event)
        {
            _defaultModuleSetupService.AddDefaultActions(@event.Id, @event.UserId);
            _defaultModuleSetupService.AddDefaultProperties(@event.Id, @event.UserId);
        }
    }
}
