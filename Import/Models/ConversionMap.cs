using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Import.Models
{
    public class ConversionMap
    {
        public string ExcelHeaderName { get; set; }
        public string PropertyKey { get; set; }
        public string DefaultValue { get; set; }
        public List<ReplacedValues> ReplacedValues { get; set; }
    }

    //public class ReplacableValue
    //{
    //    public string ExcelHeaderName { get; set; }
    //    public List<ReplacedValues> ReplacedValues { get; set; }
    //}

    public class ReplacedValues
    {
        public string ExcelValue { get; set; }
        public string SystemValue { get; set; }
    }
}
