using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Import.Models
{
    public class RowValue
    {
        public List<PropertyValues> Properties { get; set; }
    }
    public class PropertyValues
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
