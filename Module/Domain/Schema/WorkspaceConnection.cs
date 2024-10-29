namespace Module.Domain.Schema
{
    public class WorkspaceConnection
    {
        public Guid Id { get; set; }
        public Guid? SourceWorkspaceId { get; set; }
        public virtual Workspace SourceWorkspace { get; set; }
        public Guid TargetWorkspaceId { get; set; }
        public virtual Workspace TargetWorkspace { get; set; }
        public double Weight { get; set; }
        public bool IsOptional { get; set; }
        public string Metadata { get; set; }
        public bool AllowManySource { get; set; }

    }
}
