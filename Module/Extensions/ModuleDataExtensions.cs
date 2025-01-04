using Module.Domain.Data;

namespace Module.Extensions
{
    public static class ModuleDataExtensions
    {
        public static TProperty GetPropertyValue<TProperty>(
            this ModuleData moduleData,
            string propertyName)
        {
            ArgumentNullException.ThrowIfNull(moduleData);

            if (string.IsNullOrEmpty(propertyName))
                throw new ArgumentException("Property name cannot be null or empty.", nameof(propertyName));

            var propertyData = moduleData.PropertyData
                .SingleOrDefault(p => p.SystemPropertyPath == propertyName);

            if (propertyData == null)
                return default;

            var value = propertyData.GetValue();
            if (value == null)
                return default;

            return (TProperty)Convert.ChangeType(value, typeof(TProperty));
        }
    }
}
