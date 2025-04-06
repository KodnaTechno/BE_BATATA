using AppCommon.DTOs;
using Application.Interfaces;
using Events;
using Events.Modules.Workspace;

namespace Application.Features.ControlPanel.Workspace.Commands
{
    public class AssignWorkspaceModulesCommand : BaseCommand<bool>
    {
        public Guid WorkspaceId { get; set; }
        public List<Guid> ModuleIds { get; set; } = []; 

        public override IEvent GetEvent(ApiResponse<bool> response)
            => response.IsSuccess
                ? new WorkspaceModulesAssignedEvent
                {
                    WorkspaceId = WorkspaceId
                }
                : null;
    }
}
