namespace AppCommon.DTOs.Modules
{
    public class WorkspaceDto
    {
        public Guid Id { get; set; }
        public string Display {  get; set; }
        public TranslatableValue Title { get; set; }
        public string Type { get; set; }
        public DateTimeParseResult CreatedAt { get; set; }
        public DateTimeParseResult UpdatedAt { get; set; }

        public List<AppActionDto> Actions { get; set; }
        public Guid ApplicationId { get; set; }
    }
}
