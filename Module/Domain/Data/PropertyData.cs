using Module.Domain.Base;
using Module.Domain.Schema.Properties;
using Module.Domain.Shared;

namespace Module.Domain.Data
{

    public class PropertyData : BaseEntity
    {
        public Guid PropertyId { get; set; }
        public virtual Property Property { get; set; }
        public string StringValue { get; set; }
        public int? IntValue { get; set; }
        public DateTime? DateTimeValue { get; set; }
        public double? DoubleValue { get; set; }
        public decimal? DecimalValue { get; set; }
        public bool? BoolValue { get; set; }
        public object GetValue()
        {
            if (Property == null)
            {
                throw new InvalidOperationException("Property information is not available.");
            }

            return Property.DataType switch // Using DataTypeEnum here
            {
                DataTypeEnum.String => StringValue,
                DataTypeEnum.Int => IntValue,
                DataTypeEnum.DateTime => DateTimeValue,
                DataTypeEnum.Double => DoubleValue,
                DataTypeEnum.Decmial => DecimalValue,
                DataTypeEnum.Bool => BoolValue,
                DataTypeEnum.None => null,
                _ => throw new NotSupportedException($"DataType '{Property.DataType}' is not supported."),
            };
        }
    }

}
