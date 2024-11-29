    using Module.Domain.Base;
    using Module.Domain.Schema.Properties;
    using Module.Domain.Shared;
using System;

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
                DataTypeEnum.Guid => this.GuidValue = Guid.TryParse(value, out Guid val) ? val : null,
                DataTypeEnum.DateTime => this.DateTimeValue = DateTime.TryParse(value, out DateTime val) ? val : null,
                DataTypeEnum.Double => this.DoubleValue = double.TryParse(value, out double val) ? val : null,
                DataTypeEnum.Decmial => this.DecimalValue = decimal.TryParse(value, out decimal val) ? val : null,
                DataTypeEnum.Bool => this.BoolValue = bool.TryParse(value, out bool val) ? val : null,
                DataTypeEnum.None => null,
                _ => throw new NotSupportedException($"DataType '{Property.DataType}' is not supported."),
            };
        }
    }

    }
