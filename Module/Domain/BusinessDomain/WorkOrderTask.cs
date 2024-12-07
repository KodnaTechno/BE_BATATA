using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Module.Domain.BusinessDomain
{
    public class WorkOrderTask
    {
        public Guid Id { get; set; }
        public string TaskDescription { get; set; }
        public bool IsCompleted { get; set; }

        public Guid WorkOrderId { get; set; }
        public WorkOrder WorkOrder { get; set; }
    }
}
