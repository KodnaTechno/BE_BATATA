using AppWorkflow.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppWorkflow.Core.Domain.Data
{
    public class WorkflowCheckpoint
    {
        public Guid Id { get; set; }
        public Guid InstanceId { get; set; }
        public WorkflowStatus Status { get; set; }
        public Dictionary<string, object> Variables { get; set; } = new();
        public Guid CurrentStepId { get; set; }
        public DateTime CheckpointTime { get; set; }
        public Dictionary<Guid,Dictionary<string, object>> StepData { get; set; } = new();
        public string CreatedBy { get; set; }
        public string Description { get; set; }
        public bool IsAutomatic { get; set; } = true;
        
        // Navigation properties
        public virtual WorkflowData WorkflowInstance { get; set; }
    }
}
