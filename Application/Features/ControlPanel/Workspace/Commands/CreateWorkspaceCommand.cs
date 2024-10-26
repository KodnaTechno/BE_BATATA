using AppCommon.DTOs.Modules;
using Application.Common.Models;
using Application.Interfaces;
using Events;
using Events.Modules.Workspace;

namespace Application.Features.ControlPanel.Workspace.Commands
{
    public class CreateWorkspaceCommand : BaseCommand<WorkspaceDto>
    {
        public string Title { get; set; }
        public override IEvent GetEvent(ApiResponse<WorkspaceDto> response)
       => response.IsSuccess
           ? new WorkspaceCreatedEvent
           {
               Title = response.Data.Title,
               Id = response.Data.Id
           }
           : null;
    }
}
