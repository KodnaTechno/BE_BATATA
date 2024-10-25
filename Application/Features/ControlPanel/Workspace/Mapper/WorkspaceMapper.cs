using AppCommon.DTOs.Modules;
using Application.Common.Mapper;
using Application.Features.ControlPanel.Workspace.Commands;
using Riok.Mapperly.Abstractions;

namespace Application.Features.ControlPanel.Workspace.Mapping
{
    [Mapper]
    public partial class WorkspaceMapper : BaseMapper
    {
        public partial WorkspaceDto MapToDto(Module.Domain.Schema.Workspace source);

        public partial Module.Domain.Schema.Workspace MapToEntity(CreateWorkspaceCommand source);
    }
}
