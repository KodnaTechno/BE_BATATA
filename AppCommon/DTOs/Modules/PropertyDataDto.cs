using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCommon.DTOs.Modules
{
    public class PropertyDataDto
    {
        public Guid? Id { get; set; }
        public Guid PropertyId { get; set; }
        public string  Value { get; set; }
    }
}
