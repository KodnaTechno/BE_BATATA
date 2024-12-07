namespace Module.Domain.Base
{
    public abstract class SoftDeleteEntity : BaseEntity
    {
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public Guid? DeletedBy { get; set; }
    }
}
