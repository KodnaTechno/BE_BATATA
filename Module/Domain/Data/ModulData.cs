using Module.Domain.Base;

namespace Module.Domain.Data
{
    public class ModuleData: BasePropertyData
    {
        public Guid ModulId { get; set; }
        public virtual Schema.Module Module{ get; set; }

        public Guid? WorkSpaceDataId { get; set; }
        public virtual WorkspaceData? WorkspaceData{ get; set; }
    }
}
