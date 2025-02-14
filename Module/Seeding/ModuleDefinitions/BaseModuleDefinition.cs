using AppCommon;
using AppCommon.DTOs;
using AppCommon.EnumShared;
using AppCommon.GlobalHelpers;
using Module.Domain.Schema;
using Module.Domain.Schema.Properties;
using Module.Domain.Shared;
using static Module.Domain.Shared.SharedPropertyConfigurations.Common;

namespace Module.Seeding.ModuleDefinitions
{
    public abstract class BaseModuleDefinition : IModuleDefinition
    {
        protected readonly Guid _systemUserId = SystemUsers.SystemUserId;
        protected readonly DateTime _seedDate = new(2024, 1, 1);

        public abstract Domain.Schema.Module GetModule();
        public abstract IEnumerable<Property> GetProperties();

        public abstract IEnumerable<AppAction> GetBaseActions();

        protected AppAction CreateSystemAppAction(Guid Id,TranslatableValue Name,ActionType actionType, TranslatableValue description,Guid ModuleId)
        {
            return new AppAction
            {
                Id = Id,
                Name = Name,
                Type = actionType,
                Description = description,
                ModuleId = ModuleId
            };
        }

        protected Property CreateSystemProperty(
            Guid id,
            TranslatableValue title,
            string key,
            ViewTypeEnum viewType,
            DataTypeEnum dataType,
            int order,
            TranslatableValue description,
            string systemPropertyPath,
            string normalizedKey,
            Guid moduleId,
            bool isTranslatable = false,
            bool isCalculated = false)
        {
            return new Property
            {
                Id = id,
                Title = title,
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
            prefix = prefix.ToLower();
            var commonProperties = new List<Property>
            {
                CreateSystemProperty(
                    CreatedAtId,
                    CreatedAtTitle,
                    $"{prefix}_{CreatedAt.NormalizedKey.ToLower()}",
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
                    $"{prefix}_{CreatedBy.NormalizedKey.ToLower()}",
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
                    $"{prefix}_{UpdatedAt.NormalizedKey.ToLower()}",
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
                    $"{prefix}_{UpdatedBy.NormalizedKey.ToLower()}",
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
                    $"{prefix}_{DeletedAt.NormalizedKey.ToLower()}",
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
                    $"{prefix}_{DeletedBy.NormalizedKey.ToLower()}",
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
