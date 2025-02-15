using AppCommon.DTOs.Modules;
using Application.Common.Mapper;
using Riok.Mapperly.Abstractions;
using AppCommon.GlobalHelpers;

namespace Application.Features.ControlPanel.Workspace.Mapping
{
    [Mapper]
    public partial class WorkspaceMapper : BaseMapper
    {
        public WorkspaceDto MapToDto(Module.Domain.Schema.Workspace source)
        {
            return new WorkspaceDto
            {
                Id = source.Id,
                Display = source.Title.GetLocalizedValue(),
                Title = source.Title,
                Type = source.Type.ToString(),
                ApplicationId = source.ApplicationId,
                CreatedAt = MapDateTime(source.CreatedAt),
                UpdatedAt = MapDateTime(source.UpdatedAt)
            };
        }
    }
}
