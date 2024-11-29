using Module.Domain.Data;

namespace Module.Domain.Base
{
    public class BaseModuleData
    {
        public Guid ModuleDataId { get; set; }
        public virtual ModuleData ModuleData { get; set; }
    }
}
