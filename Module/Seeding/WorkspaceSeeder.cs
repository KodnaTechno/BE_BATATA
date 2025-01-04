using Microsoft.EntityFrameworkCore;
using Module.Domain.Schema;
using Module.Domain.Shared;
using Module.Domain.Schema.Properties;

namespace Module.Seeding
{
    public static class WorkspaceSeedExtensions
    {
        private static readonly Guid systemUserId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        private static readonly DateTime seedDate = new(2024, 1, 1);

        public static void SeedWorkspaces(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Workspace>().HasData(new Workspace
            {
                Id = SystemWorkspaceConstants.AssetManagement.LocationWorkspaceId,
                Title = "First Location",
                NormlizedTitle = "FIRSTLOCATION",
                Type = WorkspaceTypeEnum.Basic,
                Order = 1,
                Details = "asset location",
                ApplicationId = SystemApplicationConstants.AssetManagementApplicationId,
                CreatedAt = seedDate,
                CreatedBy = systemUserId
            });

            var properties = new List<Property>
            {
                CreateSystemProperty(
                    SystemWorkspaceConstants.AssetManagement.Properties.LocationNameId,
                    "Location Name",
                    "Location_Name",
                    ViewTypeEnum.Text,
                    DataTypeEnum.String,
                    order:1,
                    description:null,
                    systemPropertyPath:"LocationName",
                    normalizedKey:"LOCATION_NAME",
                    workspaceId: SystemWorkspaceConstants.AssetManagement.LocationWorkspaceId,
                    isTranslatable: true
                ),
                CreateSystemProperty(
                    SystemWorkspaceConstants.AssetManagement.Properties.AddressId,
                    "Address",
                    "Location_Address",
                    ViewTypeEnum.Text,
                    DataTypeEnum.String,
                    order:2,
                    description:null,
                    systemPropertyPath:"Address",
                    normalizedKey:"LOCATION_ADDRESS",
                    workspaceId: SystemWorkspaceConstants.AssetManagement.LocationWorkspaceId,
                    isTranslatable: true
                ),
                CreateSystemProperty(
                    SystemWorkspaceConstants.AssetManagement.Properties.CityId,
                    "City",
                    "Location_City",
                    ViewTypeEnum.Text,
                    DataTypeEnum.String,
                    order:3,
                    description:null,
                    systemPropertyPath:"City",
                    normalizedKey:"LOCATION_CITY",
                    workspaceId: SystemWorkspaceConstants.AssetManagement.LocationWorkspaceId,
                    isTranslatable: true
                ),
                CreateSystemProperty(
                    SystemWorkspaceConstants.AssetManagement.Properties.CountryId,
                    "Country",
                    "Location_Country",
                    ViewTypeEnum.Text,
                    DataTypeEnum.String,
                    order:4,
                    description:null,
                    systemPropertyPath:"Country",
                    normalizedKey:"LOCATION_COUNTRY",
                    workspaceId: SystemWorkspaceConstants.AssetManagement.LocationWorkspaceId,
                    isTranslatable: true
                ),
                CreateSystemProperty(
                    SystemWorkspaceConstants.AssetManagement.Properties.LatitudeId,
                    "Latitude",
                    "Location_Latitude",
                    ViewTypeEnum.Float,
                    DataTypeEnum.Decimal,
                    order:5,
                    description:null,
                    systemPropertyPath:"Latitude",
                    normalizedKey:"LOCATION_LATITUDE",
                    workspaceId: SystemWorkspaceConstants.AssetManagement.LocationWorkspaceId
                ),
                CreateSystemProperty(
                    SystemWorkspaceConstants.AssetManagement.Properties.LongitudeId,
                    "Longitude",
                    "Location_Longitude",
                    ViewTypeEnum.Float,
                    DataTypeEnum.Decimal,
                    order:6,
                    description:null,
                    systemPropertyPath:"Longitude",
                    normalizedKey:"LOCATION_LONGITUDE",
                    workspaceId: SystemWorkspaceConstants.AssetManagement.LocationWorkspaceId
                ),
                CreateSystemProperty(
                    SystemWorkspaceConstants.AssetManagement.Properties.IsActiveId,
                    "Is Active",
                    "Location_IsActive",
                    ViewTypeEnum.CheckBox,
                    DataTypeEnum.Bool,
                    order:7,
                    description:null,
                    systemPropertyPath:"IsActive",
                    normalizedKey:"LOCATION_ISACTIVE",
                    workspaceId: SystemWorkspaceConstants.AssetManagement.LocationWorkspaceId
                )
            };

            properties.AddRange(GetCommonProperties(
                SystemWorkspaceConstants.AssetManagement.LocationWorkspaceId,
                "Location",
                SystemWorkspaceConstants.AssetManagement.Properties.CreatedAtId,
                SystemWorkspaceConstants.AssetManagement.Properties.CreatedById,
                SystemWorkspaceConstants.AssetManagement.Properties.UpdatedAtId,
                SystemWorkspaceConstants.AssetManagement.Properties.UpdatedById,
                SystemWorkspaceConstants.AssetManagement.Properties.DeletedAtId,
                SystemWorkspaceConstants.AssetManagement.Properties.DeletedById,
                startingOrder: 8
            ));

            modelBuilder.Entity<Property>().HasData(properties);
        }

        private static Property CreateSystemProperty(
            Guid id,
            string title,
            string key,
            ViewTypeEnum viewType,
            DataTypeEnum dataType,
            int order,
            string description,
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

        private static IEnumerable<Property> GetCommonProperties(
            Guid workspaceId,
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
                CreateSystemProperty(CreatedAtId, "Created At", $"{prefix}_CreatedAt", ViewTypeEnum.DateTime, DataTypeEnum.DateTime, startingOrder, null, "CreatedAt", $"{prefix}_CREATEDAT", workspaceId, isCalculated:true),
                CreateSystemProperty(CreatedById, "Created By", $"{prefix}_CreatedBy", ViewTypeEnum.User, DataTypeEnum.Guid, startingOrder+1, null, "CreatedBy", $"{prefix}_CREATEDBY", workspaceId, isCalculated:true),
                CreateSystemProperty(UpdatedAtId, "Updated At", $"{prefix}_UpdatedAt", ViewTypeEnum.DateTime, DataTypeEnum.DateTime, startingOrder+2, null, "UpdatedAt", $"{prefix}_UPDATEDAT", workspaceId, isCalculated:true),
                CreateSystemProperty(UpdatedById, "Updated By", $"{prefix}_UpdatedBy", ViewTypeEnum.User, DataTypeEnum.Guid, startingOrder+3, null, "UpdatedBy", $"{prefix}_UPDATEDBY", workspaceId, isCalculated:true),
                CreateSystemProperty(DeletedAtId, "Deleted At", $"{prefix}_DeletedAt", ViewTypeEnum.DateTime, DataTypeEnum.DateTime, startingOrder+4, null, "DeletedAt", $"{prefix}_DELETEDAT", workspaceId, isCalculated:true),
                CreateSystemProperty(DeletedById, "Deleted By", $"{prefix}_DeletedBy", ViewTypeEnum.User, DataTypeEnum.Guid, startingOrder+5, null, "DeletedBy", $"{prefix}_DELETEDBY", workspaceId, isCalculated:true)
            };

            return commonProperties;
        }
    }
}
