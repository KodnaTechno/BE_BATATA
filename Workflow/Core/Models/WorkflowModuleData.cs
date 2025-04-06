using AppCommon.DTOs.Modules;
using Module.Domain.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppWorkflow.Core.Models
{
    public class WorkflowModuleData
    {
        public Guid Id { get; set; }
        public string ModuleType { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public List<PropertyDataDto> ModuleProperties { get; set; } = new();
        public PropertyDataDto GetPropertyValueByKey(string key) => ModuleProperties?.FirstOrDefault(x => x.PropertyKey is not null && x.PropertyKey.Equals(key,StringComparison.OrdinalIgnoreCase));
    }
}
