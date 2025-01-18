using AppCommon.EnumShared;

namespace AppCommon.DTOs.Modules
{
    public class PropertyDto
    {
        public string Title { get; set; }
        public string Key { get; set; }
        public string NormalizedKey { get; set; }
        public string Description { get; set; }
        public ViewTypeEnum ViewType { get; set; }
        public DataTypeEnum DataType { get; set; }
        public string Configuration { get; set; }
        public bool IsSystem { get; set; }
        public bool IsInternal { get; set; }
        public string DefaultValue { get; set; }
        public bool IsCalculated { get; set; }
        public bool IsEncrypted { get; set; }

        public bool IsTranslatable { get; set; }
        public int Order { get; set; }

        public Guid? ModuleId { get; set; }
        public Guid? WorkspaceId { get; set; }

        public Guid? WorkspaceModuleId { get; set; }
        public virtual ICollection<ValidationRuleDto> ValidationRules { get; set; }
        public string SystemPropertyPath { get; set; }
    }
}
