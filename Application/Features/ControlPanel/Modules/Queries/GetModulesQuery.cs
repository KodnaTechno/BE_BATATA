using AppCommon.DTOs.Modules;
using Application.Common.Models;
using Application.Interfaces;

namespace Application.Features.ControlPanel.Modules.Queries
{
    public class GetModulesQuery : BasePagingQuery<PaginatedList<ModuleDto>>
    {
        public Guid? ApplicationId { get; set; }
    }
}
