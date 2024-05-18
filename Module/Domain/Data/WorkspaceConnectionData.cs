using Module.Domain.Schema;

namespace Module.Domain.Data
{
    public class WorkspaceConnectionData
    {
        public Guid Id { get; set; }
        public Guid? SourceWorkspaceDataId { get; set; }
        public virtual WorkspaceData SourceWorkspaceData { get; set; }
        public Guid TargetWorkspaceDataId { get; set; }
        public virtual WorkspaceData TargetWorkspaceData { get; set; }
        public Guid WorkspaceConnectionId { get; set; }
        public virtual WorkspaceConnection WorkspaceConnection { get; set; }
    }
}
