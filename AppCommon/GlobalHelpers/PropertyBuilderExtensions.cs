using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;

namespace AppCommon.GlobalHelpers
{
    public static class PropertyBuilderExtensions
    {
        /// <summary>
        /// Configures a property to be stored as JSON in the database.
        /// </summary>
        /// <typeparam name="T">The property type.</typeparam>
        /// <param name="propertyBuilder">The <see cref="PropertyBuilder{T}"/>.</param>
        /// <param name="serializerOptions">Optional <see cref="JsonSerializerOptions"/> for serialization.</param>
        /// <returns>The <see cref="PropertyBuilder{T}"/> to allow chaining.</returns>
        public static PropertyBuilder<T> HasJsonConversion<T>(
            this PropertyBuilder<T> propertyBuilder,
            JsonSerializerOptions serializerOptions = null)
        {
            // Create the ValueConverter using System.Text.Json
            var converter = new ValueConverter<T, string>(
                v => JsonSerializer.Serialize(v, serializerOptions),
                v => JsonSerializer.Deserialize<T>(v, serializerOptions)
            );

            // Create a ValueComparer for better change tracking comparisons
            // (especially when working in memory with EF’s change tracker)
            var comparer = new ValueComparer<T>(
                (l, r) => JsonSerializer.Serialize(l, serializerOptions) == JsonSerializer.Serialize(r, serializerOptions),
                v => v == null ? 0 : JsonSerializer.Serialize(v, serializerOptions).GetHashCode(),
                v => JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(v, serializerOptions), serializerOptions)
            );

            propertyBuilder.HasConversion(converter);
            propertyBuilder.Metadata.SetValueConverter(converter);
            propertyBuilder.Metadata.SetValueComparer(comparer);

            propertyBuilder.HasColumnType("nvarchar(max)");

            return propertyBuilder;
        }
    }
}
