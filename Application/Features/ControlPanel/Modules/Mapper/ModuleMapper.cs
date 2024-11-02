using AppCommon.DTOs.Modules;
using Application.Common.Mapper;
using Application.Features.ControlPanel.Modules.Commands;
using Riok.Mapperly.Abstractions;
using Module.Domain.Schema;

namespace Application.Features.ControlPanel.Modules.Mapping
{
    [Mapper]
    public partial class ModuleMapper : BaseMapper
    {
        public partial ModuleDto MapToDto(Module.Domain.Schema.Module source);

        public partial Module.Domain.Schema.Module MapToEntity(CreateModuleCommand source);
    }
}
