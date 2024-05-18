using Module.Domain.Base;

namespace Module.Domain.Schema
{
    public class ModuleBlock : BaseEntity
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public virtual ICollection<ModuleBlockModule> ModuleBlockModules { get; set; }
    }
}
