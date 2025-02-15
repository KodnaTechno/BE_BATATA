using AppCommon.DTOs.Modules;
using Application.Interfaces;

namespace Application.Features.ControlPanel.Modules.Queries
{
    public class GetModuleQuery : BaseQuery<ModuleDto>
    {
        public Guid ModuleId { get; set; }
    }
}
