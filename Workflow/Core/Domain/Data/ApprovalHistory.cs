using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppWorkflow.Core.Domain.Data
{
    public class ApprovalHistory
    {
        public Guid Id { get; set; }
        public Guid ApprovalRequestId { get; set; }
        public string UserId { get; set; }
        public DateTime Timestamp { get; set; }
        public string Action { get; set; }
        public string Comments { get; set; }
        public Dictionary<string, object> UpdatedProperties { get; set; }
    }
}
