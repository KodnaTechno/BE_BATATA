namespace AppCommon.DTOs.Modules
{
    public class AppActionDto
    {
        public Guid Id { get; set; }
        public string DisplayName { get;set;}
        public string DisplayDescription { get;set; }
        public TranslatableValue Name { get; set; }
        public TranslatableValue Description { get; set; }
        public string Type { get; set; }

        public Guid? WorkspaceId { get; set; }
        public Guid? ModuleId { get; set; }
        public Guid? WorkspaceModuleId { get; set; }
    }
}
