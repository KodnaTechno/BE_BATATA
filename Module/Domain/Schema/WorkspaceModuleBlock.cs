namespace Module.Domain.Schema
{
    public class WorkspaceModuleBlock
    {
        public Guid Id { get; set; }
        public Guid WorkspaceId { get; set; }
        public virtual Workspace Workspace { get; set; }
        public Guid ModuleBlockId { get; set; }
        public virtual ModuleBlock Module { get; set; }
    }
}
