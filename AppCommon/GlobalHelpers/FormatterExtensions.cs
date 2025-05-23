﻿using AppCommon.DTOs.Identity;
using Localization.Resources;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace AppCommon.GlobalHelpers
{
    
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


        public static FormattedNumberValueModel FormatCurrency(this int value) => FormatCurrency((double)value);
        public static FormattedNumberValueModel FormatCurrency(this float value) => FormatCurrency((double)value);
        public static FormattedNumberValueModel FormatCurrency(this long value) => FormatCurrency((double)value);
        public static FormattedNumberValueModel FormatCurrency(this decimal value) => FormatCurrency((double)value);
        public static FormattedNumberValueModel FormatCurrency(this int? value) => FormatCurrency((double?)value);
        public static FormattedNumberValueModel FormatCurrency(this float? value) => FormatCurrency((double?)value);
        public static FormattedNumberValueModel FormatCurrency(this long? value) => FormatCurrency((double?)value);
        public static FormattedNumberValueModel FormatCurrency(this decimal? value) => FormatCurrency((double?)value);

        public static FormattedNumberValueModel FormatCurrency(this double? value)
        {
            if (value == null)
            {
                return DefaultFormattedNumberValue();
            }
            var doubleValue = (double)value;
            var Formmated = $"{value} {CurrencySymbol}".Trim();
            if (value > 1000.0)
            {
                int exp = (int)(Math.Log(doubleValue) / Math.Log(1000));
                Formmated = string.Format(Messages.Currency_Format,
                    Math.Round(doubleValue / Math.Pow(1000, exp), 2),
                    CurrencyLabels[exp - 1], CurrencySymbol).Replace(".00", "").Trim();
            }

            return new FormattedNumberValueModel
            {
                RawValue = value,
                DisplayValue = Formmated
            };
        }


        public static FormattedNumberValueModel FormatPercentageWithSymbol(this int value) => FormatPercentageWithSymbol((double)value);
        public static FormattedNumberValueModel FormatPercentageWithSymbol(this float value) => FormatPercentageWithSymbol((double)value);
        public static FormattedNumberValueModel FormatPercentageWithSymbol(this long value) => FormatPercentageWithSymbol((double)value);
        public static FormattedNumberValueModel FormatPercentageWithSymbol(this decimal value) => FormatPercentageWithSymbol((double)value);
        public static FormattedNumberValueModel FormatPercentageWithSymbol(this int? value) => FormatPercentageWithSymbol((double?)value);
        public static FormattedNumberValueModel FormatPercentageWithSymbol(this float? value) => FormatPercentageWithSymbol((double?)value);
        public static FormattedNumberValueModel FormatPercentageWithSymbol(this long? value) => FormatPercentageWithSymbol((double?)value);
        public static FormattedNumberValueModel FormatPercentageWithSymbol(this decimal? value) => FormatPercentageWithSymbol((double?)value);

        public static FormattedNumberValueModel FormatPercentageWithSymbol(this double? value)
        {
            if (value == null)
            {
                return DefaultFormattedNumberValue();
            }
            return new FormattedNumberValueModel
            {
                RawValue = value,
                DisplayValue = $"{value / 100:P1}".Replace(".0", "")
            };
        }

        public static FormattedNumberValueModel FormatPercentage(this int value) => FormatPercentage((double)value);
        public static FormattedNumberValueModel FormatPercentage(this float value) => FormatPercentage((double)value);
        public static FormattedNumberValueModel FormatPercentage(this long value) => FormatPercentage((double)value);
        public static FormattedNumberValueModel FormatPercentage(this decimal value) => FormatPercentage((double)value);
        public static FormattedNumberValueModel FormatPercentage(this int? value) => FormatPercentage((double?)value);
        public static FormattedNumberValueModel FormatPercentage(this float? value) => FormatPercentage((double?)value);
        public static FormattedNumberValueModel FormatPercentage(this long? value) => FormatPercentage((double?)value);
        public static FormattedNumberValueModel FormatPercentage(this decimal? value) => FormatPercentage((double?)value);

        public static FormattedNumberValueModel FormatPercentage(this double? value)
        {
            if (value == null)
            {
                return DefaultFormattedNumberValue();
            }
            return new FormattedNumberValueModel
            {
                RawValue = value,
                DisplayValue = $"{value / 100:P1}".Replace(".0", "")
            };
        }

        public static FormattedNumberValueModel FormatNumber(this int value) => FormatNumber((double)value);
        public static FormattedNumberValueModel FormatNumber(this float value) => FormatNumber((double)value);
        public static FormattedNumberValueModel FormatNumber(this long value) => FormatNumber((double)value);
        public static FormattedNumberValueModel FormatNumber(this decimal value) => FormatNumber((double)value);
        public static FormattedNumberValueModel FormatNumber(this int? value) => FormatNumber((double?)value);
        public static FormattedNumberValueModel FormatNumber(this float? value) => FormatNumber((double?)value);
        public static FormattedNumberValueModel FormatNumber(this long? value) => FormatNumber((double?)value);
        public static FormattedNumberValueModel FormatNumber(this decimal? value) => FormatNumber((double?)value);

        public static FormattedNumberValueModel FormatNumber(this double? money)
        {
            if (money == null)
            {
                return DefaultFormattedNumberValue();
            }
            var doubleMoney = (double)money;
            const int decimals = 2;
            string result = money.ToString();

            foreach (Suffixes suffix in Enum.GetValues(typeof(Suffixes)))
            {
                double currentVal = Math.Pow(10, (int)suffix * 3);
                string suff = Enum.GetName(typeof(Suffixes), (int)suffix) ?? string.Empty;

                if (money >= currentVal)
                    result = $"{Math.Round(doubleMoney / currentVal, decimals)}{suff}";
                else
                    return new FormattedNumberValueModel
                    {
                        RawValue = money,
                        DisplayValue = result
                    };

            }

            return new FormattedNumberValueModel
            {
                RawValue = money,
                DisplayValue = result
            };
        }

        public static FormattedNumberValueModel DefaultFormattedNumberValue()
        {
            return new FormattedNumberValueModel
            {
                RawValue = null,
                DisplayValue = "--"
            };
        }
    }
}
