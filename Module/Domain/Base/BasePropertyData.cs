using Module.Domain.Data;

namespace Module.Domain.Base
{
    public class BasePropertyData : SoftDeleteEntity
    {
        public virtual ICollection<PropertyData> PropertyData { get; set; }
    }
}
