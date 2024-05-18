using Module.Domain.Data;

namespace Module.Domain.Base
{
    public class BasePropertyData: BaseEntity
    {
        public virtual ICollection<PropertyData> ProperatyData { get; set; }
    }
}
