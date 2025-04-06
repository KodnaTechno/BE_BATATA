using AppCommon.DTOs;
using AppCommon.DTOs.Modules;
using Application.Interfaces;

namespace Application.Features.ControlPanel.Workspace.Queries
{
    public class GetWorkspacesQuery : BasePagingQuery<PaginatedList<WorkspaceDto>>
    {
        public Guid ApplicationId { get; set; }
    }
}
