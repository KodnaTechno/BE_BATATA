using AppCommon.DTOs.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppWorkflow.Services.Interfaces
{
    public interface IPropertiesProvider
    {
        List<PropertyDto> GetProperties(List<string> PropertiesGUIDs);
    }
}
