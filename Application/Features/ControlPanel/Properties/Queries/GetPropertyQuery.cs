using AppCommon.DTOs.Modules;
using Application.Interfaces;

namespace Application.Features.ControlPanel.Properties.Queries
{
    public class GetPropertyQuery : BaseQuery<PropertyDto>
    {
        public Guid PropertyId { get; set; }
    }
}
