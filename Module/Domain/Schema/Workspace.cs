using AppCommon.DTOs;
using AppCommon.EnumShared;
using Module.Domain.Base;
using Module.Domain.Data;

namespace Module.Domain.Schema
{
    public class Workspace : BaseProperty
    {
        public TranslatableValue Title { get; set; }
        public WorkspaceTypeEnum Type { get; set; }
        public string Key { get; set; }
        public int Order { get; set; }
        public TranslatableValue Details { get; set; }
        public Guid ApplicationId { get; set; }
        public Application Application { get; set; }
        public virtual ICollection<WorkspaceModule> WorkspaceModules { get; set; }
        public virtual ICollection<WorkspaceModuleBlock> WorkspaceModuleBlocks { get; set; }
        public virtual ICollection<WorkspaceData> WorkspaceData {  get; set; }
        public virtual ICollection<AppAction> Actions { get; set; }    
    }
}
