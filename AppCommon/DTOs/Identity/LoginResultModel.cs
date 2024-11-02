using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCommon.DTOs.Identity
{
    public class LoginResultModel
    {
        public bool Successful { get; set; }
        public string Error { get; set; }
        public string Token { get; set; }
        public Guid UserId { get; set; }
    }
}
