using Module.Domain.Schema.Properties;

namespace Module.Domain.Base
{
    public class BaseProperty : SoftDeleteEntity
    {
        public virtual ICollection<Property> Properties { get; set; }
    }
}
