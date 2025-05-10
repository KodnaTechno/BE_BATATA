using AppWorkflow.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppWorkflow.Core.Domain.Data
{
    public class WorkflowRelation
    {
        public Guid Id { get; set; }
        public Guid ParentInstanceId { get; set; }
        public Guid ChildInstanceId { get; set; }
        public WorkflowRelationType RelationType { get; set; }
        public string Name { get; set; }
        public Dictionary<string, string> Metadata { get; set; } = new();
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CompletedAt { get; set; }

        // Navigation properties
        public virtual WorkflowData ParentInstance { get; set; }
        public virtual WorkflowData ChildInstance { get; set; }
    }
}
