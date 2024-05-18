using Module.Domain.Base;
using Module.Domain.Shared;

namespace Module.Domain.Schema
{
    public class Module : BaseModule
    {
        public string Title { get; set; }
        public ModuleTypeEnum Type { get; set; }
        public string Name { get; set; }
        public string Domain { get; set; }
        public bool IsActive { get; set; }
        public int Order { get; set; }
        public string Details { get; set; }
        public virtual ICollection<WorkspaceModule> WorkspaceModules { get; set; }
    }
}
