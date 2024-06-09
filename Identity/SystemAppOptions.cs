using AppIdentity.Domain;
using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppIdentity
{
    public class SystemAppOptions : AuthenticationSchemeOptions
    {
        public List<AppCredential>? RegisteredApps { get; set; }
    }

}
