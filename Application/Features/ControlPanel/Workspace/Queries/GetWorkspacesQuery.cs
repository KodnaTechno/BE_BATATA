using AppCommon.DTOs.Modules;
using Application.Common.Models;
using Application.Interfaces;

namespace Application.Features.ControlPanel.Workspace.Queries
{
    public class GetWorkspacesQuery : BasePagingQuery<PaginatedList<WorkspaceDto>>
    {
        public string SearchTerm { get; set; }
    }
}
