namespace Module.Domain.Schema
{
    public class ModuleBlockModule
    {
        public Guid Id { get; set; }
        public Guid ModuleBlockId { get; set; }
        public virtual ModuleBlock ModuleBlock { get; set; }
        public Guid ModuleId { get; set; }
        public virtual Module Module { get; set; }
    }
}
