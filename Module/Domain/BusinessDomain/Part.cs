using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Module.Domain.BusinessDomain
{
    public class Part
    {
        public Guid Id { get; set; }
        public string EquipmentName { get; set; }
        public string SerialNumber { get; set; }
        public int AssetId { get; set; }
        public Asset Asset { get; set; }

        // Navigation Properties
        public ICollection<WorkOrder> WorkOrders { get; set; }
        public ICollection<MaintenanceSchedule> MaintenanceSchedules { get; set; }
    }
}
