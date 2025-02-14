using AppCommon.DTOs;
using System.Globalization;

namespace AppCommon.GlobalHelpers
{
    public static class TranslatableValueExtensions
    {
        public static string GetLocalizedValue(this TranslatableValue value)
        {
            if (value == null) return string.Empty;

            var currentCulture = CultureInfo.CurrentCulture.Name.ToLower();

            return currentCulture.StartsWith("ar")
                ? value.Ar ?? value.En ?? string.Empty
                : value.En ?? value.Ar ?? string.Empty;
        }

        public static string GetLocalizedValue(this TranslatableValue value, string cultureName)
        {
            if (value == null) return string.Empty;

            return cultureName.ToLower().StartsWith("ar")
                ? value.Ar ?? value.En ?? string.Empty
                : value.En ?? value.Ar ?? string.Empty;
        }
    }
}
