namespace Module.Domain.Base
{
    public class BaseEntity
    {
        public Guid Id { get; set; }
        public Guid? UpdatedBy { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}
