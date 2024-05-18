using Module.Domain.Base;

namespace Module.Domain.Data
{
    public class WorkspaceData : BasePropertyData
    {
        public virtual ICollection<WorkspaceConnectionData> WorkspaceConnections { get; set; }
    }

}
