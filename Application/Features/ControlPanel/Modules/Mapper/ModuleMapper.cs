using AppCommon.DTOs.Modules;
using Application.Common.Mapper;
using Riok.Mapperly.Abstractions;
using AppCommon.GlobalHelpers;

namespace Application.Features.ControlPanel.Modules.Mapping
{
    [Mapper]
    public partial class ModuleMapper : BaseMapper
    {
        public ModuleDto MapToDto(Module.Domain.Schema.Module source)
        {
            if (source == null)
                return null;

            return new ModuleDto
            {
                Id = source.Id,
                Display = source.Title.GetLocalizedValue(),
                Title = source.Title,
                Type = source.Type.ToString(),
                Key = source.Key,
                Domain = source.Domain,
                IsActive = source.IsActive,
                Order = source.Order,
                Details = source.Details,
                ApplicationId = source.ApplicationId
            };
        }
    }
}
