using AppCommon.EnumShared;
using Module.Domain.Base;

namespace Module.Domain.Schema
{
    public class Module : BaseProperty
    {
        public string Title { get; set; }
        public ModuleTypeEnum Type { get; set; }
        public string Name { get; set; }
        public string Domain { get; set; }
        public bool IsActive { get; set; }
        public int Order { get; set; }
        public string Details { get; set; }
        public Guid? ApplicationId { get; set; }
        public Application Application { get; set; }
        public virtual ICollection<WorkspaceModule> WorkspaceModules { get; set; }
    }
}
