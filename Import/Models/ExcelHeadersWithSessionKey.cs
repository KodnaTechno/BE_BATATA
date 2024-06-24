using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Import.Models
{
    public class ExcelHeadersWithSessionKey
    {
        public string FileSessionKey { get; set; }
        public List<string> Headers { get; set; }
    }
}
