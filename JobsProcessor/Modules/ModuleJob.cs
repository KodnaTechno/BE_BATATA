using Application.Services.DefaultSetupService;
using Events.Modules.Modules;
using MediatR;

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
           var addedActions= _defaultModuleSetupService.AddDefaultActions(@event.Id, @event.UserId);
            _defaultModuleSetupService.AddDefaultProperties(@event.Id, @event.UserId);
            _defaultModuleSetupService.AddDefaultWorkflows(addedActions, @event.UserId);
        }
    }
}
