namespace Module.Domain.Base
{
    public abstract class BaseEntity
    {
        public Guid Id { get; set; }
        public Guid? UpdatedBy { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
