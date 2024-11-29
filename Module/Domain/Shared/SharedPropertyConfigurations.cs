using AppCommon.DTOs;
using Module.Domain.Base;

namespace Module.Domain.Shared
{
    public static class SharedPropertyConfigurations
    {
        public static class Common
        {
            public static readonly TranslatableValue CreatedAtTitle = new() { En = "Created At", Ar = "تاريخ الإنشاء" };
            public static readonly TranslatableValue CreatedByTitle = new() { En = "Created By", Ar = "تم الإنشاء بواسطة" };
            public static readonly TranslatableValue UpdatedAtTitle = new() { En = "Updated At", Ar = "تاريخ التحديث" };
            public static readonly TranslatableValue UpdatedByTitle = new() { En = "Updated By", Ar = "تم التحديث بواسطة" };
            public static readonly TranslatableValue DeletedAtTitle = new() { En = "Deleted At", Ar = "تاريخ الحذف" };
            public static readonly TranslatableValue DeletedByTitle = new() { En = "Deleted By", Ar = "تم الحذف بواسطة" };

            public static class CreatedAt
            {
                public const string NormalizedKey = "CREATED_AT";
                public const ViewTypeEnum ViewType = ViewTypeEnum.DateTime;
                public const DataTypeEnum DataType = DataTypeEnum.DateTime;
                public const string SystemPropertyPath = nameof(BaseEntity.CreatedAt);
                public const int DefaultOrder = 1;
            }

            public static class CreatedBy
            {
                public const string NormalizedKey = "CREATED_BY";
                public const ViewTypeEnum ViewType = ViewTypeEnum.User;
                public const DataTypeEnum DataType = DataTypeEnum.Guid;
                public const string SystemPropertyPath = nameof(BaseEntity.CreatedBy);
                public const int DefaultOrder = 2;
            }

            public static class UpdatedAt
            {
                public const string NormalizedKey = "UPDATED_AT";
                public const ViewTypeEnum ViewType = ViewTypeEnum.DateTime;
                public const DataTypeEnum DataType = DataTypeEnum.DateTime;
                public const string SystemPropertyPath = nameof(BaseEntity.UpdatedAt);
                public const int DefaultOrder = 3;
            }

            public static class UpdatedBy
            {
                public const string NormalizedKey = "UPDATED_BY";
                public const ViewTypeEnum ViewType = ViewTypeEnum.User;
                public const DataTypeEnum DataType = DataTypeEnum.Guid;
                public const string SystemPropertyPath = nameof(BaseEntity.UpdatedBy);
                public const int DefaultOrder = 4;
            }

            public static class DeletedAt
            {
                public const string NormalizedKey = "DELETED_AT";
                public const ViewTypeEnum ViewType = ViewTypeEnum.DateTime;
                public const DataTypeEnum DataType = DataTypeEnum.DateTime;
                public const string SystemPropertyPath = nameof(SoftDeleteEntity.DeletedAt);
                public const int DefaultOrder = 5;
            }

            public static class DeletedBy
            {
                public const string NormalizedKey = "DELETED_BY";
                public const ViewTypeEnum ViewType = ViewTypeEnum.User;
                public const DataTypeEnum DataType = DataTypeEnum.Guid;
                public const string SystemPropertyPath = nameof(SoftDeleteEntity.DeletedBy);
                public const int DefaultOrder = 6;
            }
        }
    }
}
