using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Module.Domain.BusinessDomain
{
    public class WorkOrder
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public int EquipmentId { get; set; }
        public Part Equipment { get; set; }

        public int AssignedToUserId { get; set; } // Nullable if unassigned
        public Guid AssignedToUser { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime? DueDate { get; set; }
        
        //public WorkOrderStatus Status { get; set; }

        // Navigation Properties
        public ICollection<WorkOrderTask> WorkOrderTasks { get; set; }
       // public ICollection<WorkOrderInventoryItem> WorkOrderInventoryItems { get; set; }
        public ICollection<Instruction> Instructions { get; set; }
        //public ICollection<Document> Documents { get; set; }
    }
}
