using AppCommon.DTOs;
using AppCommon.GlobalHelpers;
using Module.Domain.Schema.Properties;
using Module.Domain.Shared;
using static Module.Domain.Shared.SharedPropertyConfigurations.Common;

namespace Module.Seeding.ModuleDefinitions
{
    public abstract class BaseModuleDefinition : IModuleDefinition
    {
        protected readonly Guid _systemUserId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        protected readonly DateTime _seedDate = new(2024, 1, 1);

        public abstract Domain.Schema.Module GetModule();
        public abstract IEnumerable<Property> GetProperties();

        protected Property CreateSystemProperty(
            Guid id,
            TranslatableValue title,
            string key,
            ViewTypeEnum viewType,
            DataTypeEnum dataType,
            int order,
            string description,
            string systemPropertyPath,
            string normalizedKey,
            Guid moduleId,
            bool isTranslatable = false,
            bool isCalculated = false)
        {
            return new Property
            {
                Id = id,
                Title = title.AsText(),
                Key = key.ToLower(),
                NormalizedKey = normalizedKey.ToUpper(),
                Description = description,
                ViewType = viewType,
                DataType = dataType,
                IsSystem = true,
                IsInternal = true,
                IsTranslatable = isTranslatable,
                IsCalculated = isCalculated,
                Order = order,
                SystemPropertyPath = systemPropertyPath,
                ModuleId = moduleId,
                CreatedAt = _seedDate,
                CreatedBy = _systemUserId
            };
        }

        protected IEnumerable<Property> GetCommonProperties(Guid moduleId,
            string prefix,
            Guid CreatedAtId,
            Guid CreatedById,
            Guid UpdatedAtId,
            Guid UpdatedById,
            Guid DeletedAtId,
            Guid DeletedById,
            int startingOrder = 1)
        {
            var commonProperties = new List<Property>
            {
                CreateSystemProperty(
                    CreatedAtId,
                    CreatedAtTitle,
                    $"{prefix}_CreatedAt",
                    CreatedAt.ViewType,
                    CreatedAt.DataType,
                    startingOrder,
                    null,
                    CreatedAt.SystemPropertyPath,
                    CreatedAt.NormalizedKey,
                    moduleId,
                    isCalculated:true
                ),
                CreateSystemProperty(
                    CreatedById,
                    CreatedByTitle,
                    $"{prefix}_CreatedBy",
                    CreatedBy.ViewType,
                    CreatedBy.DataType,
                    startingOrder + 1,
                    null,
                    CreatedBy.SystemPropertyPath,
                    CreatedBy.NormalizedKey,
                    moduleId,
                    isCalculated:true
                ),
                CreateSystemProperty(
                    UpdatedAtId,
                    UpdatedAtTitle,
                    $"{prefix}_UpdatedAt",
                    UpdatedAt.ViewType,
                    UpdatedAt.DataType,
                    startingOrder + 2,
                    null,
                    UpdatedAt.SystemPropertyPath,
                    UpdatedAt.NormalizedKey,
                    moduleId,
                    isCalculated:true
                ),
                CreateSystemProperty(
                    UpdatedById,
                    UpdatedByTitle,
                    $"{prefix}_UpdatedBy",
                    UpdatedBy.ViewType,
                    UpdatedBy.DataType,
                    startingOrder + 3,
                    null,
                    UpdatedBy.SystemPropertyPath,
                    UpdatedBy.NormalizedKey,
                    moduleId,
                    isCalculated:true
                ),
                CreateSystemProperty(
                    DeletedAtId,
                    DeletedAtTitle,
                    $"{prefix}_DeletedAt",
                    DeletedAt.ViewType,
                    DeletedAt.DataType,
                    startingOrder + 4,
                    null,
                    DeletedAt.SystemPropertyPath,
                    DeletedAt.NormalizedKey,
                    moduleId,
                    isCalculated:true
                ),
                CreateSystemProperty(
                    DeletedById,
                    DeletedByTitle,
                    $"{prefix}_DeletedBy",
                    DeletedBy.ViewType,
                    DeletedBy.DataType,
                    startingOrder + 5,
                    null,
                    DeletedBy.SystemPropertyPath,
                    DeletedBy.NormalizedKey,
                    moduleId,
                    isCalculated:true
                )
            };

            commonProperties.ForEach(p => p.ModuleId = moduleId);
            return commonProperties;
        }
    }
}
