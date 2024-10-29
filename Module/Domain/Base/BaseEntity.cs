namespace Module.Domain.Base
{
    public class BaseEntity
    {
        public Guid Id { get; set; }
        public Guid? UpdatedBy { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid? DeletedBy { get; set; }
        public DateTime? DeletedAt { get;set; }
        public bool IsDeleted { get; set; }
    }
}
