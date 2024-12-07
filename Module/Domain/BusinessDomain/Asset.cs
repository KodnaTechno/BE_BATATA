using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Module.Domain.BusinessDomain
{
    public class Asset
    {
        public Guid Id{ get; set; }
        public string AssetName { get; set; }
        public string AssetTag { get; set; } 
        public int LocationId { get; set; }

        // Navigation Properties
        public ICollection<Part> Equipments { get; set; }
        public ICollection<Asset> ChildrenAssets{ get; set; }
        public ICollection<MaintenanceSchedule> MaintenanceSchedules { get; set; }
    }
}
