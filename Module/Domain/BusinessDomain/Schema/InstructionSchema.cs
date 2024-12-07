using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Module.Domain.BusinessDomain.Schema
{
    public class InstructionSchema
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public Guid? ParentId { get;set; }
        public InstructionSchema Parent { get; set; }
        public ICollection<InstructionSchema> Children { get; set; }
        public Guid MSId{ get; set; }
        public MaintenanceSchedule MS { get; set; }
    }
}
