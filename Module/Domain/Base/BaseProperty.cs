using Module.Domain.Schema.Properties;

namespace Module.Domain.Base
{
    public class BaseProperty : BaseEntity
    {
        public virtual ICollection<Property> Properaties { get; set; }
    }
}
