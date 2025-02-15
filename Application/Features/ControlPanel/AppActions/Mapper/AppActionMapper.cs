using AppCommon.DTOs.Modules;
using AppCommon.GlobalHelpers;
using Application.Common.Mapper;
using Module.Domain.Schema;
using Riok.Mapperly.Abstractions;

namespace Application.Features.ControlPanel.AppActions.Mapper
{
    [Mapper]
    public partial class AppActionMapper : BaseMapper
    {
        public AppActionDto MapToDto(AppAction source)
        {
            if (source == null) return null;

            var dto = new AppActionDto
            {
                Id = source.Id,
                Type = source.Type.ToString(),
                WorkspaceId = source.WorkspaceId,
                ModuleId = source.ModuleId,
                WorkspaceModuleId = source.WorkspaceModuleId,
                Name = source.Name,
                DisplayName = source.Name.GetLocalizedValue(),
                Description = source.Description,
                DisplayDescription = source.Description.GetLocalizedValue(),
            };
            return dto;
        }
    }
}
