namespace AppCommon.DTOs.Modules
{
    public class AppActionDto
    {
        public Guid Id { get; set; }
        public string Display {get;set;}
        public TranslatableValue Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
    }
}
