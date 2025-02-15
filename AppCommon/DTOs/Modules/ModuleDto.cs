using AppCommon.EnumShared;

namespace AppCommon.DTOs.Modules
{
    public class ModuleDto
    {
        public Guid Id { get; set; }
        public string Display { get; set; }
        public TranslatableValue Title { get; set; }
        public string Type { get; set; }
        public string Key { get; set; }
        public string Domain { get; set; }
        public bool IsActive { get; set; }
        public int Order { get; set; }
        public TranslatableValue Details { get; set; }
        public Guid? ApplicationId { get; set; }
    }
}
