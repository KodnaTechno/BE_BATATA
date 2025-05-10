using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppWorkflow.Core.Domain.Data
{
    public class SynchronizationPoint
    {
        public Guid Id { get; set; }
        public Guid WorkflowInstanceId { get; set; }
        public List<Guid> RequiredStepIds { get; set; } = new();
        public List<Guid> CompletedStepIds { get; set; } = new();
        public string Name { get; set; }
        public bool IsMet => RequiredStepIds.Count == CompletedStepIds.Count;
        public DateTime? CompletedAt { get; set; }
    }
}
