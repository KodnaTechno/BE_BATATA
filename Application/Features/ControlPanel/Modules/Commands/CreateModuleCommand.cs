using AppCommon.DTOs.Modules;
using Application.Common.Models;
using Application.Interfaces;
using Events;
using Events.Modules.Modules;

namespace Application.Features.ControlPanel.Modules.Commands
{
    public class CreateModuleCommand : BaseCommand<ModuleDto>
    {
        public string Title { get; set; }
        public override IEvent GetEvent(ApiResponse<ModuleDto> response)
       => response.IsSuccess
           ? new ModuleCreatedEvent
           {
               Id = response.Data.Id
           }
           : null;
    }
}
