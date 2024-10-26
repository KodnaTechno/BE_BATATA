using AppCommon.DTOs.Modules;
using Application.Interfaces;

namespace Application.Features.ControlPanel.Workspace.Queries
{
    public class GetWorkspaceQuery : BaseQuery<WorkspaceDto>
    {
        public Guid WorkspaceId { get; set; }
    }
}
