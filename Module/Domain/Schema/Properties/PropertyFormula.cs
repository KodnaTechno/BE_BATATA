using Module.Domain.Base;

namespace Module.Domain.Schema.Properties
{
    public class PropertyFormula : BaseEntity
    {
        public Guid PropertyId { get; set; }
        public virtual Property Property { get; set; }
        public string Formula { get; set; }
        public string Value { get; set; }
        public bool IsConditional { get; set; }

        public int Order { get; set; }
    }
}
