namespace Infrastructure.Database.Domain
{
    public class AppConfig
    {
        public Guid Id { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public DateTime LastUpdated { get; set; }
        public Guid? UpdatedBy { get; set; }
    }
}
