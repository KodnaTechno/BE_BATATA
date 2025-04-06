

namespace AppCommon.DTOs.Modules.PropertyConfigs
{
    public class ConnectionOptionDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
    }

    public class ConnectionConfig
    {
        public bool AllowMany { get; set; }
        public bool IsOptional { get; set; }
        public List<ConnectionOptionDto> Options { get; set; }
    }
}
