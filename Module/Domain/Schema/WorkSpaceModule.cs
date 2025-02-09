using Module.Domain.Base;

namespace Module.Domain.Schema
{
    public class WorkspaceModule : BaseProperty
    {
        public Guid WorkspaceId { get; set; }
        public virtual Workspace Workspace { get; set; }
        public Guid ModuleId { get; set; }
        public virtual Module Module { get; set; }
        public virtual ICollection<AppAction> Actions { get; set; }
    }
}
