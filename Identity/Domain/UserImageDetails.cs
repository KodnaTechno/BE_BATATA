using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppIdentity.Domain
{
    public class UserImageDetails
    {
        public byte[] Binary { get; set; }
        public string ImageReference { get; set; }
        public bool IsStream { get; set; }
    }
}
