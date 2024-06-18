namespace AppCommon.DTOs
{
    public class TimezoneSetting
    {
        public Region Region { get; set; }
        public string DateFormat { get; set; }
        public string TimeFormat { get; set; }

    }
    public class Region
    {
        public string Id { get; set; }
        public string BaseUtcOffset { set; get; }
        public string StandardName { get; set; }
        public string DisplayName { get; set; }
        public string DaylightName { get; set; }
        public string Name { get; set; }
    }
}
