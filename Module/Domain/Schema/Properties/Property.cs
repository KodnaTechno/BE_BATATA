using AppCommon.DTOs;
using AppCommon.EnumShared;
using Module.Domain.Base;

namespace Module.Domain.Schema.Properties
{
    public class Property : SoftDeleteEntity
    {
        public TranslatableValue Title { get; set; }
        public string Key { get; set; }
        public string NormalizedKey { get; set; }
        public TranslatableValue Description { get; set; }
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

        public Guid? ApplicationId { get; set; }
        public virtual Application Application { get; set; }

        public virtual ICollection<PropertyFormula> PropertyFormulas { get; set; }
        public virtual ICollection<ValidationRule> ValidationRules { get; set; }

        public virtual ICollection<PropertyConnection> SourcePropertyConnections { get; set; }
        public virtual ICollection<PropertyConnection> TargetPropertyConnections { get; set; }
        public string SystemPropertyPath { get; set; }
    }
}
