using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Module.Domain.BusinessDomain
{
    public class Instruction
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime CompletionDate { get; set; }
        public Guid? ParentId { get; set; }
        public Instruction Parent { get; set; }
        public ICollection<Instruction> Children { get; set; }
        public Guid TaskId { get; set; }


    }
}
