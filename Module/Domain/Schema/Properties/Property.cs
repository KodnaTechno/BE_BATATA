using Module.Domain.Base;
using Module.Domain.Shared;

namespace Module.Domain.Schema.Properties
{
    public class Property : SoftDeleteEntity
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
        public virtual Module Module { get; set; }
        public Guid? WorkspaceId { get; set; }
        public virtual Workspace Workspace { get; set; }

        public Guid? WorkspaceModuleId { get; set; }
        public virtual WorkspaceModule WorkspaceModule { get; set; }
        public virtual ICollection<PropertyFormula> PropertyFormulas { get; set; }
        public virtual ICollection<ValidationRule> ValidationRules { get; set; }

        public virtual ICollection<PropertyConnection> SourcePropertyConnections { get; set; }
        public virtual ICollection<PropertyConnection> TargetPropertyConnections { get; set; }
        public string SystemPropertyPath { get; set; }
    }
}
