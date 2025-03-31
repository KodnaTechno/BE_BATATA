using AppCommon.DTOs;
using AppCommon.DTOs.Modules;
using Application.Interfaces;
using Events;
using Events.Modules.Workspace;

namespace Application.Features.ControlPanel.Workspace.Commands
{
    public class CreateWorkspaceCommand : BaseCommand<WorkspaceDto>
    {
        public TranslatableValue Title { get; set; }
        public Guid ApplicationId { get;  set; }
        public TranslatableValue Details { get;  set; }

        public override IEvent GetEvent(ApiResponse<WorkspaceDto> response)
       => response.IsSuccess
           ? new WorkspaceCreatedEvent
           {
               Id = response.Data.Id
           }
           : null;
    }
}
