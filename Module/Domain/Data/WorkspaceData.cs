using Module.Domain.Base;
using Module.Domain.Schema;

namespace Module.Domain.Data
{
    public class WorkspaceData : BasePropertyData
    {
        public virtual ICollection<WorkspaceConnectionData> WorkspaceConnections { get; set; }
        public Guid WorkspaceId { get;set; }
        public Workspace Workspace { get; set; }
    }

}
