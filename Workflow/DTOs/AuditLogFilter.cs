namespace AppWorkflow.Core.DTOs;

using System.Text;

public class AuditLogFilter
{
    public string EntityType { get; set; }
    public Guid? EntityId { get; set; }
    public string UserId { get; set; }
    public List<AuditAction> Actions { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    public int Skip { get; set; } = 0;
    public int Take { get; set; } = 50;
}