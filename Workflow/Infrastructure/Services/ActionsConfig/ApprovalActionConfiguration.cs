using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppWorkflow.Infrastructure.Services.Actions
{
    public class ApprovalActionConfiguration
    {
        public string Type { get; set; } // "Role" or "Group" or "User"
        public string RoleId { get; set; }
        public string GroupId { get; set; }
        public string UserId { get; set; }
        public bool UseTargetResolver { get; set; }
        public string TargetResolverType { get; set; }
        public List<ApprovalPropertyConfig> EditableProperties { get; set; } = new();
        public string ApprovalMessage { get; set; }
        public int TimeoutInHours { get; set; }
        public string SuccessTransition { get; set; }
        public string RejectTransition { get; set; }
        public string TimeoutTransition { get; set; }
    }
    public class ApprovalPropertyConfig
    {
        public string PropertyName { get; set; }
        public bool IsRequired { get; set; }
        public string ValidationExpression { get; set; }
        public bool IsReadOnly { get; set; }
    }
}
