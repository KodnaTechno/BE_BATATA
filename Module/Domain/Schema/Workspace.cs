using Module.Domain.Base;
using Module.Domain.Data;
using Module.Domain.Shared;

namespace Module.Domain.Schema
{
    public class Workspace : BaseProperty
    {
        public string Title { get; set; }
        public WorkspaceTypeEnum Type { get; set; }
        public string NormlizedTitle { get; set; }
        public int Order { get; set; }
        public string Details { get; set; }
        public Guid ApplicationId { get; set; }
        public Application Application { get; set; }
        public virtual ICollection<WorkspaceModule> WorkspaceModules { get; set; }
        public virtual ICollection<WorkspaceModuleBlock> WorkspaceModuleBlocks { get; set; }
        public virtual ICollection<WorkspaceData> WorkspaceData {  get; set; }
    }
}
