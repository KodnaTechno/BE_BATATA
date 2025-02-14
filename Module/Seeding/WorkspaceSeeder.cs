using Microsoft.EntityFrameworkCore;
using Module.Domain.Schema;
using Module.Domain.Schema.Properties;
using AppCommon.DTOs;
using AppCommon.EnumShared;
using static Module.Domain.Shared.SharedPropertyConfigurations.Common;
using AppCommon;

namespace Module.Seeding
{
    public static class WorkspaceSeedExtensions
    {
        private static readonly Guid systemUserId = SystemUsers.SystemUserId;
        private static readonly DateTime seedDate = new(2024, 1, 1);

        public static void SeedWorkspaces(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Workspace>().HasData(new Workspace
            {
                Id = SystemWorkspaceConstants.ProjectManagement.LocationWorkspaceId,
                Title = new TranslatableValue { Ar = "مشروع", En = "Project" },
                Key = "PROJECT",
                Type = WorkspaceTypeEnum.Basic,
                Order = 1,
                Details = new TranslatableValue { Ar = "مشروع", En = "Project" },
                ApplicationId = SystemApplicationConstants.ProjectManagementApplicationId,
                CreatedAt = seedDate,
                CreatedBy = systemUserId
            });

            var properties = new List<Property>();

            properties.AddRange(GetCommonProperties(
                SystemWorkspaceConstants.ProjectManagement.LocationWorkspaceId,
                "Project",
                SystemWorkspaceConstants.ProjectManagement.Properties.CreatedAtId,
                SystemWorkspaceConstants.ProjectManagement.Properties.CreatedById,
                SystemWorkspaceConstants.ProjectManagement.Properties.UpdatedAtId,
                SystemWorkspaceConstants.ProjectManagement.Properties.UpdatedById,
                SystemWorkspaceConstants.ProjectManagement.Properties.DeletedAtId,
                SystemWorkspaceConstants.ProjectManagement.Properties.DeletedById,
                startingOrder: 8
            ));

            modelBuilder.Entity<Property>().HasData(properties);
        }

        private static Property CreateSystemProperty(
            Guid id,
            TranslatableValue title,
            string key,
            ViewTypeEnum viewType,
            DataTypeEnum dataType,
            int order,
            TranslatableValue description,
            string systemPropertyPath,
            string normalizedKey,
            Guid workspaceId,
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
                WorkspaceId = workspaceId,
                CreatedAt = seedDate,
                CreatedBy = systemUserId
            };
        }


        private static IEnumerable<Property> GetCommonProperties(Guid workspaceId,
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
                    workspaceId,
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
                    workspaceId,
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
                    workspaceId,
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
                    workspaceId,
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
                    workspaceId,
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
                    workspaceId,
                    isCalculated:true
                )
            };

            return commonProperties;
        }
    }
}
