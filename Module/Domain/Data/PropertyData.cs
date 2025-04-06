using AppCommon.EnumShared;
using Module.Domain.Base;
using Module.Domain.Schema.Properties;

namespace Module.Domain.Data
{
    public class PropertyData : BaseEntity
        {
            public Guid PropertyId { get; set; }
            public virtual Property Property { get; set; }
            public DataTypeEnum DataType { get; set; }
            public ViewTypeEnum ViewType  { get; set; }
            public string SystemPropertyPath { get; set; } // To Map the System Property 

            public Guid? GuidValue { get; set; }
            public string StringValue { get; set; }
            public int? IntValue { get; set; }
            public DateTime? DateTimeValue { get; set; }
            public DateOnly? DateValue { get; set; }
            public double? DoubleValue { get; set; }
            public decimal? DecimalValue { get; set; }
            public bool? BoolValue { get; set; }

            public Guid? ModuleDataId { get; set; }
            public virtual ModuleData ModuleData { get; set; }

            public Guid? WorkspaceDataId { get; set; }
            public virtual WorkspaceData WorkspaceData { get; set; }

            public object GetValue()
            {
                return DataType switch
                {
                    DataTypeEnum.Guid => GuidValue.Value,
                    DataTypeEnum.String => StringValue,
                    DataTypeEnum.Int => IntValue.Value,
                    DataTypeEnum.DateTime => DateTimeValue.Value,
                    DataTypeEnum.DateOnly => DateValue.Value,
                    DataTypeEnum.Double => DoubleValue.Value,
                    DataTypeEnum.Decimal => DecimalValue.Value,
                    DataTypeEnum.Bool => BoolValue.Value,
                    DataTypeEnum.None => null,
                    _ => throw new NotSupportedException($"DataType '{DataType}' is not supported."),
                };
            }

            public object SetValue(string value)
            {
                return DataType switch
                {
                    DataTypeEnum.String => StringValue = value,
                    DataTypeEnum.Int => IntValue = int.TryParse(value, out int val) ? val : null,
                    DataTypeEnum.Guid => GuidValue = Guid.TryParse(value, out Guid val) ? val : null,
                    DataTypeEnum.DateTime => DateTimeValue = DateTime.TryParse(value, out DateTime val) ? val : null,
                    DataTypeEnum.DateOnly => DateValue = DateTime.TryParse(value, out DateTime val) ? DateOnly.FromDateTime(val) : null,
                    DataTypeEnum.Double => DoubleValue = double.TryParse(value, out double val) ? val : null,
                    DataTypeEnum.Decimal => DecimalValue = decimal.TryParse(value, out decimal val) ? val : null,
                    DataTypeEnum.Bool => BoolValue = bool.TryParse(value, out bool val) ? val : null,
                    DataTypeEnum.None => null,
                    _ => throw new NotSupportedException($"DataType '{DataType}' is not supported."),
                };
            }
        }

    }
