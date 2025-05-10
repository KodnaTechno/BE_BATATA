using AppWorkflow.Core.Models;
using AppWorkflow.Infrastructure.Services.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppWorkflow.Core.Domain.Data
{
    public class ApprovalRequest
    {
        public Guid Id { get; set; }
        public Guid WorkflowDataId { get; set; }
        public Guid StepDataId { get; set; }
        public List<string> ApproverIds { get; set; }
        public List<ApprovalPropertyConfig> EditableProperties { get; set; }
        public WorkflowModuleData ModuleData { get; set; }
        public string ApprovalMessage { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public ApprovalStatus Status { get; set; }
        public string ProcessedBy { get; set; }
        public DateTime? ProcessedAt { get; set; }
        public string Comments { get; set; }
        public Dictionary<string, object> UpdatedProperties { get; set; }

        public string CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
    }
}
