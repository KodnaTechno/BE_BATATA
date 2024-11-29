    using Module.Domain.Base;
    using Module.Domain.Schema.Properties;
    using Module.Domain.Shared;

    namespace Module.Domain.Data
    {
        public class PropertyData : BaseEntity
        {
            public Guid PropertyId { get; set; }
            public virtual Property Property { get; set; }

            public DataTypeEnum DataType { get; set; }
            public string SystemPropertyPath { get; set; } // To Map the System Property 

            public Guid? GuidValue { get; set; }
            public string StringValue { get; set; }
            public int? IntValue { get; set; }
            public DateTime? DateTimeValue { get; set; }
            public DateOnly? DateValue { get; set; }
            public double? DoubleValue { get; set; }
            public decimal? DecimalValue { get; set; }
            public bool? BoolValue { get; set; }

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
                    DataTypeEnum.Decmial => DecimalValue.Value,
                    DataTypeEnum.Bool => BoolValue.Value,
                    DataTypeEnum.None => null,
                    _ => throw new NotSupportedException($"DataType '{Property.DataType}' is not supported."),
                };
            }
        }

    }
