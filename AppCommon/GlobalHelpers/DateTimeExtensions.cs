using AppCommon.DTOs;
using Microsoft.Extensions.Options;
using System.Globalization;

namespace AppCommon.GlobalHelpers
{
    public static class DateTimeExtensions
    {
        private static TimezoneSetting _timezoneSetting = new(); 

        public static void ConfigureTimezone(IOptionsMonitor<TimezoneSetting> timezoneSettings)
        {
            timezoneSettings.OnChange(settings =>
            {
                _timezoneSetting = settings;
            });

            _timezoneSetting = timezoneSettings.CurrentValue;
        }

        private const string EMPTY_DISPLAY = "-";
        public const string DATE_FORMAT = "yyyy-MM-dd";
        public const string DATETIME_FORMAT = "yyyy-MM-dd hh:mm tt";
        private const string TIME_FORMAT = "hh:mm tt";

        public static DateTimeParseResult FormatDateTime(this object dateTime, bool getDisplayValue = false)
        {
            return Parse((DateTime)dateTime, includeTime: true);
        }


        public static DateTimeParseResult FormatDateTime(this DateTime dateTime)
        {
            return Parse(dateTime, includeTime: true);
        }

        public static DateTimeParseResult FormatDate(this DateTime dateTime)
        {
            return Parse(dateTime, includeTime: false);
        }

        public static DateTimeParseResult FormatDateTime(this DateTime? dateTime)
        {
            return Parse(dateTime, includeTime: true);
        }

        public static DateTimeParseResult FormatDate(this DateTime? dateTime)
        {
            return Parse(dateTime, includeTime: false);
        }

        public static object FormatTime(this TimeSpan? time)
        {
            CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
            TimeSpan? regionTime = time.AdjustToRegionTime();
            string format = _timezoneSetting.TimeFormat;
            string timespanFormat = ConvertDateTimeFormatToTimeSpanFormat(format);
            string displayValue = "00:00";
            if (regionTime.HasValue)
                displayValue = DateTime.MinValue.Add(regionTime.Value).ToString(timespanFormat, cultureInfo);

            return new
            {
                DisplayValue = displayValue,
                RawValue = regionTime
            };
        }


        public static object FormatDate(this string value)
        {
            return ParseStringDate(value, DATE_FORMAT, dt => dt.FormatDate());
        }

        public static object FormatDateTime(this string value)
        {
            return ParseStringDate(value, DATETIME_FORMAT, dt => dt.FormatDateTime());
        }

        public static object FormatTime(this string value)
        {
            return ParseStringDate(value, TIME_FORMAT, dt => FormatTime(dt.TimeOfDay));
        }

        public static string ToSystemDateString(this string value)
        {
            var parsedResult = DateTime.Parse(value, CultureInfo.InvariantCulture);
            return parsedResult.ToString(DATE_FORMAT);
        }

        public static string ToSystemDateTimeString(this string value)
        {
            var parsedResult = DateTime.Parse(value, CultureInfo.InvariantCulture);
            return parsedResult.ToString(DATETIME_FORMAT);
        }

        public static string ToSystemTimeString(this string value)
        {
            var parsedResult = DateTime.Parse(value, CultureInfo.InvariantCulture);
            return parsedResult.ToString(TIME_FORMAT);
        }

        public static string ToUtcDateString(this string value) => ToUtcDate(value, true).ToString(DATE_FORMAT);


        public static string ToUtcDateTimeString(this string value) => ToUtcDateTime(value).ToString(DATETIME_FORMAT);

        public static DateTime ToUtcDate(this string value, bool removeTimePart = false)
        {
            if (removeTimePart)
            {
                if (value.Contains(' '))
                {
                    string[] parts = value.Split(' ');
                    value = parts[0];
                }
            }
            if (value == null)
            {
                return DateTime.MinValue;
            }
            var parsedResult = value.ParseStringDate(DATE_FORMAT, dt => dt);
            if (parsedResult is DateTime dateTime)
            {
                return dateTime;
            }
            else
            {
                throw new FormatException($"Failed to parse date-time string: {value}");
            }
        }
        public static DateTime ToUtcDateTime(this string value)
        {
            var parsedResult = value.ParseStringDate(DATETIME_FORMAT, dt => dt);

            if (parsedResult is DateTime dateTime)
            {
                DateTime utcDateTime = dateTime.ToUTC();
                return utcDateTime;
            }
            else
            {
                throw new FormatException($"Failed to parse date-time string: {value}");
            }
        }
        public static string ToUtcTimeString(this string value)
        {
            var parsedResult = value.ParseStringDate(TIME_FORMAT, dt => dt.TimeOfDay);

            if (parsedResult is TimeSpan timeSpan)
            {
                TimeSpan utcAdjustedTime = timeSpan.Subtract(GetBaseUtcOffset());
                if (utcAdjustedTime.Days > 0) utcAdjustedTime = utcAdjustedTime.Subtract(new TimeSpan(1, 0, 0, 0));
                if (utcAdjustedTime.Days < 0) utcAdjustedTime = utcAdjustedTime.Add(new TimeSpan(1, 0, 0, 0));
                DateTime dateTime = new DateTime(1, 1, 2).Add(utcAdjustedTime);


                return dateTime.ToString(TIME_FORMAT);
            }
            else
            {
                throw new FormatException($"Failed to parse time string: {value}");
            }
        }


        public static DateTime? ToUTC(this DateTime? dateTime, bool isDateOnly = false)
        {
            if (!dateTime.HasValue)
                return null;

            return isDateOnly ? dateTime : AdjustToUTC(dateTime.Value);
        }

        public static DateTime ToUTC(this DateTime dateTime, bool isDateOnly = false)
        {
            return isDateOnly ? dateTime : AdjustToUTC(dateTime);
        }

        public static string ToDateTimString(this DateTime dateTime)
        {
            return dateTime.ToString(DATETIME_FORMAT);
        }
        public static string ToDateString(this DateTime dateTime)
        {
            return dateTime.ToString(DATE_FORMAT);
        }

        private static DateTimeParseResult Parse(DateTime? dateTime, bool includeTime)
        {
            if (!dateTime.HasValue || dateTime.Value == DateTime.MinValue)
            {
                return new DateTimeParseResult();
            }

            CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
            DateTime? adjustedDateTime;
            _timezoneSetting.DateFormat = _timezoneSetting.DateFormat.CorrectDateFormat();

            string displayValue;

            if (includeTime)
            {
                adjustedDateTime = dateTime.AdjustToRegionDateTime();
                string format = $"{_timezoneSetting.DateFormat} {_timezoneSetting.TimeFormat}";
                displayValue = adjustedDateTime?.ToString(format, cultureInfo);
            }
            else
            {
                adjustedDateTime = dateTime;
                displayValue = adjustedDateTime?.ToString(_timezoneSetting.DateFormat, cultureInfo);
            }

            var actualValue = includeTime ? adjustedDateTime : adjustedDateTime?.Date;

            return new DateTimeParseResult
            {
                DisplayValue = displayValue,
                RawValue = actualValue.Value
            };
        }


        private static object ParseStringDate(this string value, string format, Func<DateTime, object> parseFunc)
        {
            if (string.IsNullOrEmpty(value)) return FailedParseResult();

            if (DateTime.TryParseExact(value, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDateTime))
            {
                return parseFunc(parsedDateTime);
            }

            return parseFunc(DateTime.MinValue);
        }

        private static DateTimeParseResult FailedParseResult(DateTime? actualValue = null)
        {
            return new DateTimeParseResult { DisplayValue = EMPTY_DISPLAY, RawValue = actualValue.Value };
        }

        private static TimeSpan GetBaseUtcOffset()
        {
            string baseUtcOffsetString = _timezoneSetting.Region?.BaseUtcOffset ?? TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow).ToString();
            if (!TimeSpan.TryParse(baseUtcOffsetString, out TimeSpan offset))
            {
                throw new InvalidOperationException($"Invalid UTC offset format: {baseUtcOffsetString}");
            }

            return offset;
        }

        public static DateTime AdjustToRegionDateTime(this DateTime dateTime)
        {
            return dateTime.Add(GetBaseUtcOffset());
        }

        public static DateTime? AdjustToRegionDateTime(this DateTime? dateTime)
        {
            return dateTime?.Add(GetBaseUtcOffset());
        }

        private static TimeSpan? AdjustToRegionTime(this TimeSpan? time)
        {
            return time.HasValue ? new TimeSpan(time.Value.Ticks + GetBaseUtcOffset().Ticks) : (TimeSpan?)null;
        }

        private static DateTime AdjustToUTC(DateTime dateTime)
        {
            TimeSpan offset = GetBaseUtcOffset();

            if (offset > TimeSpan.Zero)
            {
                return dateTime.Subtract(offset);
            }
            else
            {
                return dateTime.Add(-offset);
            }
        }

        private static string CorrectDateFormat(this string dateFormat)
        {
            if (string.IsNullOrWhiteSpace(dateFormat))
                return dateFormat;

            // Check if "mm" is surrounded by year or day indicators
            if (dateFormat.Contains("yyyy-mm") || dateFormat.Contains("dd-mm"))
                return dateFormat.Replace("mm", "MM");

            return dateFormat;
        }

        private static string ConvertDateTimeFormatToTimeSpanFormat(string dateTimeFormat)
        {
            if (string.IsNullOrEmpty(dateTimeFormat))
                return dateTimeFormat;

            return dateTimeFormat.Replace(":", @"\:");

        }

        public static int ToBusinessWorkingDays(this DateTime start, DateTime due, int[] weekends, List<(DateTime from, DateTime to)> holidays)
        {
            if (start > due)
            {
                return -1; // Return an error code or throw an exception as needed.
            }

            // Preprocess holidays into a hash set for faster checking
            var holidayDates = new HashSet<DateTime>();
            foreach (var (from, to) in holidays)
            {
                for (DateTime date = from; date <= to; date = date.AddDays(1))
                {
                    holidayDates.Add(date);
                }
            }

            // Count business days excluding weekends and holidays
            int businessDays = Enumerable.Range(0, (due - start).Days + 1)
                .Select(offset => start.AddDays(offset))
                .Count(date => !weekends.Contains((int)date.DayOfWeek) && !holidayDates.Contains(date));

            return businessDays;
        }
    }
}
