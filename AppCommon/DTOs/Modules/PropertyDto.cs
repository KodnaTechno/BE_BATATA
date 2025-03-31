namespace AppCommon.DTOs.Modules
{
    public class PropertyDto
    {
        public Guid Id { get; set; }
        public TranslatableValue Title { get; set; }
        public string Display { get; set; }
        public string Key { get; set; }
        public string NormalizedKey { get; set; }
        public TranslatableValue Description { get; set; }
        public string DescriptionDisplay { get; set; }
        public string ViewType { get; set; }
        public string DataType { get; set; }
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
        public Guid? ApplicationId { get; set; }
        public DateTimeParseResult CreatedAt { get; set; }
        public DateTimeParseResult UpdatedAt { get; set; }
    }

}
