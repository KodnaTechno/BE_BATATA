using AppCommon.EnumShared;

namespace Module.Service
{
    public static class PropertyTypeMapper
    {
        /// <summary>
        /// Maps ViewType to appropriate DataType based on the field's presentation needs
        /// </summary>
        public static DataTypeEnum GetDataTypeForViewType(ViewTypeEnum viewType)
        {
            return viewType switch
            {
                // String-based ViewTypes
                ViewTypeEnum.Text => DataTypeEnum.String,
                ViewTypeEnum.LongText => DataTypeEnum.String,
                ViewTypeEnum.RichText => DataTypeEnum.String,
                ViewTypeEnum.ExternalDisplayValue => DataTypeEnum.String,

                // Numeric ViewTypes
                ViewTypeEnum.Int => DataTypeEnum.Int,
                ViewTypeEnum.Float => DataTypeEnum.Decimal,
                ViewTypeEnum.Currency => DataTypeEnum.Decimal,
                ViewTypeEnum.Percentage => DataTypeEnum.Decimal,

                // Date-time ViewTypes
                ViewTypeEnum.Date => DataTypeEnum.DateOnly,
                ViewTypeEnum.Time => DataTypeEnum.DateTime,
                ViewTypeEnum.DateTime => DataTypeEnum.DateTime,
                ViewTypeEnum.Month => DataTypeEnum.DateOnly,
                ViewTypeEnum.Week => DataTypeEnum.DateOnly,
                ViewTypeEnum.Quarter => DataTypeEnum.DateOnly,

                // Reference ViewTypes
                ViewTypeEnum.User => DataTypeEnum.Guid,
                ViewTypeEnum.MultiUser => DataTypeEnum.String, // JSON array of GUIDs
                ViewTypeEnum.Lookup => DataTypeEnum.Guid,
                ViewTypeEnum.MultiLookup => DataTypeEnum.String, // JSON array of GUIDs
                ViewTypeEnum.ModuleReference => DataTypeEnum.String, // JSON reference data

                // Boolean ViewTypes
                ViewTypeEnum.CheckBox => DataTypeEnum.Bool,

                // Complex ViewTypes
                ViewTypeEnum.Api => DataTypeEnum.String, // Usually stores configuration or result
                ViewTypeEnum.MappedPropertyList => DataTypeEnum.String, // JSON structure
                ViewTypeEnum.Dynamic => DataTypeEnum.String, // JSON structure
                ViewTypeEnum.Attachment => DataTypeEnum.String, // File reference or metadata
                ViewTypeEnum.MultiAttachment => DataTypeEnum.String, // JSON array of file references

                // Default fallback
                _ => DataTypeEnum.String
            };
        }

        /// <summary>
        /// Determines if a ViewType typically stores multiple values
        /// </summary>
        public static bool IsMultiValueType(ViewTypeEnum viewType)
        {
            return viewType switch
            {
                ViewTypeEnum.MultiUser => true,
                ViewTypeEnum.MultiLookup => true,
                ViewTypeEnum.MultiAttachment => true,
                ViewTypeEnum.MappedPropertyList => true,
                _ => false
            };
        }

        /// <summary>
        /// Determines if a ViewType typically requires configuration
        /// </summary>
        public static bool RequiresConfiguration(ViewTypeEnum viewType)
        {
            return viewType switch
            {
                ViewTypeEnum.Lookup => true,
                ViewTypeEnum.MultiLookup => true,
                ViewTypeEnum.Api => true,
                ViewTypeEnum.ModuleReference => true,
                ViewTypeEnum.MappedPropertyList => true,
                ViewTypeEnum.Dynamic => true,
                _ => false
            };
        }

        /// <summary>
        /// Provides default configuration for ViewTypes that require it
        /// </summary>
        public static string GetDefaultConfiguration(ViewTypeEnum viewType)
        {
            return viewType switch
            {
                ViewTypeEnum.ModuleReference => "{\"ReferenceModuleId\":\"\",\"Multiple\":false}",
                ViewTypeEnum.Lookup => "{\"SourceModuleId\":\"\",\"DisplayProperty\":\"\"}",
                ViewTypeEnum.MultiLookup => "{\"SourceModuleId\":\"\",\"DisplayProperty\":\"\"}",
                ViewTypeEnum.Api => "{\"Endpoint\":\"\",\"Method\":\"GET\"}",
                ViewTypeEnum.MappedPropertyList => "{\"Properties\":[]}",
                ViewTypeEnum.Dynamic => "{\"Type\":\"\",\"Config\":{}}",
                _ => null
            };
        }
    }
}
