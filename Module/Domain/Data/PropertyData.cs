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

        public object SetValue(string value)
        {
            if (Property == null)
            {
                throw new InvalidOperationException("Property information is not available.");
            }

            return Property.DataType switch // Using DataTypeEnum here
            {
                DataTypeEnum.String => this.StringValue = value,
                DataTypeEnum.Int => this.IntValue = int.TryParse(value, out int val) ? val : null,
                DataTypeEnum.DateTime => this.DateTimeValue = DateTime.TryParse(value, out DateTime val) ? val : null,
                DataTypeEnum.Double => this.DoubleValue = double.TryParse(value, out double val) ? val : null,
                DataTypeEnum.Decmial => this.DecimalValue = decimal.TryParse(value, out decimal val) ? val : null,
                DataTypeEnum.Bool => this.BoolValue = bool.TryParse(value, out bool val) ? val : null,
                DataTypeEnum.None => null,
                _ => throw new NotSupportedException($"DataType '{Property.DataType}' is not supported."),
            };
        }

        public Guid? ModuleDataId{ get; set; }
        public virtual ModuleData? ModuleData { get; set; }

        public Guid? WorkspaceDataId { get; set; }
        public virtual WorkspaceData WorkspaceData { get; set; }

    }

}
