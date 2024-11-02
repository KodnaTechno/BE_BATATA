using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCommon.DTOs.Modules
{
    public class ModuleDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        //public ModuleTypeEnum Type { get; set; }
        public string Name { get; set; }
        public string Domain { get; set; }
        public bool IsActive { get; set; }
        public int Order { get; set; }
        public string Details { get; set; }
        public Guid? ApplicationId { get; set; }
    }
}
