namespace AppCommon.DTOs
{
    public class DateTimeParseResult
    {
        public string DisplayValue {  get; set; }
        public DateTime RawValue { get; set; }
    }

    public class TimeSpanParseResult
    {
        public string DisplayValue { get; set; }
        public TimeSpan RawValue { get; set; }
    }
}
