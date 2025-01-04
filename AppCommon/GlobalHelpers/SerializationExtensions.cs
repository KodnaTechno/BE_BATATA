using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;

namespace AppCommon.GlobalHelpers
{
    public static class SerializationExtensions
    {
        // Configuration options for JsonSerializerOptions
        private static readonly JsonSerializerOptions options = new JsonSerializerOptions
        {
            WriteIndented = true, // For readable JSON; set to false for smaller JSON size
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull, // To ignore null values in serialization
            IgnoreReadOnlyProperties = false, // Whether to ignore read-only properties
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase, // To convert property names to camelCase
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping // To relax JSON escaping
        };
        public static string AsText(this object? obj)
        {
            try
            {
                if (obj == null) return string.Empty;
                if (obj is string _str) return _str;
                if (obj is DateTime _date) return _date.ToString();
                // Serialize object to JSON string with specified options
                return JsonSerializer.Serialize(obj, options);
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                Console.WriteLine($"Error during serialization: {ex.Message}");
                throw ex;
            }
        }
        public static T FromJson<T>(this string jsonString)
        {
            if (string.IsNullOrEmpty(jsonString))
            {
                return default;
            }

            try
            {
                return JsonSerializer.Deserialize<T>(jsonString, options);
            }
            catch (JsonException ex)
            {
                // Handle or log the JSON parsing error
                Console.WriteLine($"JSON deserialization error: {ex.Message}");
                throw; // Re-throwing the exception to let the caller handle it or fail the operation.
            }
        }
        public static JsonDocument Clone(this JsonDocument original)
        {
            // Serialize the JsonDocument to a string
            string jsonString = original.RootElement.GetRawText();

            // Parse the string back into a new JsonDocument
            return JsonDocument.Parse(jsonString);
        }
        public static T CloneObj<T>(this T o) 
        {
            return FromJson<T>(AsText(o));
        }
    }
}
