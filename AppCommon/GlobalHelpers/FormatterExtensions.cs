using Localization.Resources;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace AppCommon.GlobalHelpers
{
    public class FormattedNumberValue
    {
        public double? Value { get; set; }
        public string Formatted { get; set; }
    }

    public static class FormatterExtensions
    {
        // TODO: Make this dynamic after finding a way to read from DB settings and support distributed servers.
        public static string CurrencySymbol = "$";

        public enum Suffixes
        {
            p, // Placeholder for values under 1 thousand.
            K, // Thousand
            M, // Million
            B, // Billion
            T, // Trillion
            Q  // Quadrillion
        }

        private static readonly List<string> CurrencyLabels = new List<string>
        {
            Messages.Currency_K,
            Messages.Currency_M,
            Messages.Currency_G,
            Messages.Currency_T,
            Messages.Currency_P,
            Messages.Currency_E
        };


        public static FormattedNumberValue FormatCurrency(this int value) => FormatCurrency((double)value);
        public static FormattedNumberValue FormatCurrency(this float value) => FormatCurrency((double)value);
        public static FormattedNumberValue FormatCurrency(this long value) => FormatCurrency((double)value);
        public static FormattedNumberValue FormatCurrency(this decimal value) => FormatCurrency((double)value);

        public static FormattedNumberValue FormatCurrency(this double value)
        {
            var Formmated = $"{value} {CurrencySymbol}".Trim();
            if (value > 1000.0)
            {
                int exp = (int)(Math.Log(value) / Math.Log(1000));
                Formmated = string.Format(Messages.Currency_Format,
                    Math.Round(value / Math.Pow(1000, exp), 2),
                    CurrencyLabels[exp - 1], CurrencySymbol).Replace(".00", "").Trim();
            }

            return new FormattedNumberValue
            {
                Value = value,
                Formatted = Formmated
            };
        }


        public static FormattedNumberValue FormatPercentageWithSymbol(this int value) => FormatPercentageWithSymbol((double)value);
        public static FormattedNumberValue FormatPercentageWithSymbol(this float value) => FormatPercentageWithSymbol((double)value);
        public static FormattedNumberValue FormatPercentageWithSymbol(this long value) => FormatPercentageWithSymbol((double)value);
        public static FormattedNumberValue FormatPercentageWithSymbol(this decimal value) => FormatPercentageWithSymbol((double)value);

        public static FormattedNumberValue FormatPercentageWithSymbol(this double value)
        {
            return new FormattedNumberValue
            {
                Value = value,
                Formatted = $"{value / 100:P1}".Replace(".0", "")
            };
        }

        public static FormattedNumberValue FormatPercentage(this int value) => FormatPercentage((double)value);
        public static FormattedNumberValue FormatPercentage(this float value) => FormatPercentage((double)value);
        public static FormattedNumberValue FormatPercentage(this long value) => FormatPercentage((double)value);
        public static FormattedNumberValue FormatPercentage(this decimal value) => FormatPercentage((double)value);

        public static FormattedNumberValue FormatPercentage(this double value)
        {
            return new FormattedNumberValue
            {
                Value = value,
                Formatted = $"{value / 100:P1}".Replace(".0", "")
            };
        }

        public static FormattedNumberValue FormatNumber(this int value) => FormatNumber((double)value);
        public static FormattedNumberValue FormatNumber(this float value) => FormatNumber((double)value);
        public static FormattedNumberValue FormatNumber(this long value) => FormatNumber((double)value);
        public static FormattedNumberValue FormatNumber(this decimal value) => FormatNumber((double)value);

        public static FormattedNumberValue FormatNumber(this double money)
        {
            const int decimals = 2;
            string result = money.ToString();

            foreach (Suffixes suffix in Enum.GetValues(typeof(Suffixes)))
            {
                double currentVal = Math.Pow(10, (int)suffix * 3);
                string suff = Enum.GetName(typeof(Suffixes), (int)suffix) ?? string.Empty;

                if (money >= currentVal)
                    result = $"{Math.Round(money / currentVal, decimals)}{suff}";
                else
                    return new FormattedNumberValue
                    {
                        Value = money,
                        Formatted = result
                    };

            }

            return new FormattedNumberValue
            {
                Value = money,
                Formatted = result
            };
        }
    }
}
