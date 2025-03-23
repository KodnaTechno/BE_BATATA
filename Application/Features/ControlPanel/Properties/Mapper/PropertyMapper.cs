using AppCommon.DTOs.Modules;
using AppCommon.GlobalHelpers;
using Application.Common.Mapper;
using Module.Domain.Schema.Properties;
using Riok.Mapperly.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.ControlPanel.Properties.Mapper
{
    [Mapper]
    public partial class PropertyMapper : BaseMapper
    {
        public PropertyDto MapToDto(Property source)
        {
            return new PropertyDto
            {
                Id = source.Id,
                Title = source.Title,
                Display = source.Title.GetLocalizedValue(),
                Key = source.Key,
                NormalizedKey = source.NormalizedKey,
                Description = source.Description,
                DescriptionDisplay = source.Description.GetLocalizedValue(),
                ViewType = source.ViewType.ToString(),
                DataType = source.DataType.ToString(),
                Configuration = source.Configuration,
                IsSystem = source.IsSystem,
                IsInternal = source.IsInternal,
                DefaultValue = source.DefaultValue,
                IsCalculated = source.IsCalculated,
                IsEncrypted = source.IsEncrypted,
                IsTranslatable = source.IsTranslatable,
                Order = source.Order,
                ModuleId = source.ModuleId,
                WorkspaceId = source.WorkspaceId,
                WorkspaceModuleId = source.WorkspaceModuleId,
                ApplicationId = source.ApplicationId,
                SystemPropertyPath = source.SystemPropertyPath,
                CreatedAt = MapDateTime(source.CreatedAt),
                UpdatedAt = MapDateTime(source.UpdatedAt)
            };
        }
    }
}
