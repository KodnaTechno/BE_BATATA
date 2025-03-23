using AppCommon.DTOs.Modules;
using Application.Common.Models;
using Application.Interfaces;
using MediatR;

namespace Application.Features.ControlPanel.Properties.Queries
{
    public class GetPropertiesQuery : BasePagingQuery<PaginatedList<PropertyDto>>
    {
        public Guid? ApplicationId { get; set; }
        public Guid? WorkspaceId { get; set; }
        public Guid? ModuleId { get; set; }
        public Guid? WorkspaceModuleId { get; set; }
    }
}
