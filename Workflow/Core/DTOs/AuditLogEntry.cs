namespace AppWorkflow.Core.DTOs;

using AppWorkflow.Common.Enums;
using System.Text;

public class AuditLogEntry
{
    public string EntityType { get; set; }
    public Guid EntityId { get; set; }
    public AuditAction Action { get; set; }
    public string UserId { get; set; }
    public string UserName { get; set; }
    public object OldValues { get; set; }
    public object NewValues { get; set; }
    public Dictionary<string, string> Metadata { get; set; }
    public string Notes { get; set; }
}