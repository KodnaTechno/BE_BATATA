using AppCommon.EnumShared.BusinessEnums;
using Module.Domain.BusinessDomain.Schema;

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
