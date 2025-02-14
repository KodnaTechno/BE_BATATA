using AppCommon.DTOs.Modules;
using Application.Common.Models;
using Application.Interfaces;
using Events;
using Events.Modules.Workspace;

namespace Application.Features.ControlPanel.Workspace.Commands
{
    public class SetWorkspaceModulesCommand : BaseCommand<WorkspaceDto>
    {
        public Guid WorkspaceId { get; set; }
        public List<Guid> ModuleIds { get;set; }
        public override IEvent GetEvent(ApiResponse<WorkspaceDto> response)
       => response.IsSuccess
           ? new WorkspaceModulesSetEvent
           {
               Id = response.Data.Id
           }
           : null;
    }
}
