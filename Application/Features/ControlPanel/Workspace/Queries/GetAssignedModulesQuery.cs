using AppCommon.DTOs;
using AppCommon.DTOs.Modules;
using Application.Interfaces;

namespace Application.Features.ControlPanel.Workspace.Queries
{
    public class GetAssignedModulesQuery : BasePagingQuery<PaginatedList<WorkspaceModuleDto>>
    {
        public Guid WorkspaceId { get; set; }
    }
}
