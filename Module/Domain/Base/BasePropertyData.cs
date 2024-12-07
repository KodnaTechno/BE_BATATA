using Module.Domain.Data;

namespace Module.Domain.Base
{
    public class BasePropertyData : SoftDeleteEntity
    {
        public virtual ICollection<PropertyData> ProperatyData { get; set; }
    }
}
