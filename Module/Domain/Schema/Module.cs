using AppCommon.DTOs;
using AppCommon.EnumShared;
using Module.Domain.Base;

namespace Module.Domain.Schema
{
    public class Module : BaseProperty
    {
        public TranslatableValue Title { get; set; }
        public ModuleTypeEnum Type { get; set; }
        public string Key { get; set; }
        public string Domain { get; set; }
        public bool IsActive { get; set; }
        public int Order { get; set; }
        public TranslatableValue Details { get; set; }
        public Guid? ApplicationId { get; set; }
        public Application Application { get; set; }
        public virtual ICollection<WorkspaceModule> WorkspaceModules { get; set; }
        public virtual ICollection<AppAction> Actions { get; set; }

    }
}
