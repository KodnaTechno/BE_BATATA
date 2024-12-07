using Module.Domain.BusinessDomain.Schema;
using Module.Domain.Shared.BusinessEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Module.Domain.Shared.BusinessEnums.MaintenanceFrequencyEnum;

namespace Module.Domain.BusinessDomain
{
    public class MaintenanceSchedule
    {
        public Guid Id { get; set; }
        public string ScheduleName { get; set; }

        public int AssetId { get; set; }
        public Asset Asset { get; set; }

        public WorkOrderSchema workOrderSchema { get; set; }
        public Guid WorkOrderSchemaId { get; set; }
        public MaintenanceFrequencyEnum Frequency { get; set; }
        public DateTime NextScheduledDate { get; set; }

        // Navigation Properties
        public ICollection<WorkOrder> GeneratedWorkOrders { get; set; }
    }
}
