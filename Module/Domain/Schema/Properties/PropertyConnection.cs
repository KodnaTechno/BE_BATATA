using Module.Domain.Base;
using Module.Domain.Shared;

namespace Module.Domain.Schema.Properties
{
    public class PropertyConnection : BaseEntity
    {
        public Guid SourcePropertyId { get; set; }
        public virtual Property SourceProperty { get; set; }

        public Guid TargetPropertyId { get; set; }
        public virtual Property TargetProperty { get; set; }

        public ConnectionType ConnectionType { get; set; }
    }
}
