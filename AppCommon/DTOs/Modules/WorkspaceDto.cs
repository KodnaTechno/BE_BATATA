namespace AppCommon.DTOs.Modules
{
    public class WorkspaceDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public DateTimeParseResult CreatedAt { get; set; }
        public DateTimeParseResult UpdatedAt { get; set; }
    }
}
