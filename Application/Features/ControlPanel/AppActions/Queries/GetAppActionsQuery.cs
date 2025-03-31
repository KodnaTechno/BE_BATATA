using AppCommon.DTOs;
using AppCommon.DTOs.Modules;
using Application.Interfaces;

namespace Application.Features.ControlPanel.AppActions.Queries
{
    public class GetActionsQuery : BasePagingQuery<PaginatedList<AppActionDto>>
    {
        public Guid? WorkspaceId { get; set; }
        public Guid? ModuleId { get; set; }
        public Guid? WorkspaceModuleId { get; set; }
    }
}
