namespace AppWorkflow.Core.Domain.Data;

using AppWorkflow.Common.Enums;
using System.Security.Principal;
using System.Text;
using System.Text.Json;

public class AuditLog
{
    public Guid Id { get; set; }
    public string EntityType { get; set; }
    public Guid EntityId { get; set; }
    public AuditAction Action { get; set; }
    public string UserId { get; set; }
    public string UserName { get; set; }
    public DateTime Timestamp { get; set; }
    public JsonDocument OldValues { get; set; }
    public JsonDocument NewValues { get; set; }
    public Dictionary<string, string> Metadata { get; set; } = new();
    public string IpAddress { get; set; }
    public string UserAgent { get; set; }
    public string Notes { get; set; }

    // IEntity implementation
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string UpdatedBy { get; set; }
}