using System.Text.Json;

namespace AppWorkflow.Infrastructure.Triggers;

public class TriggerContext
    {
        public string ModuleType { get; set; }
        public Guid ModuleId { get; set; }
        public string TriggerType { get; set; }
        public JsonDocument Data { get; set; }
        public Dictionary<string, object> Metadata { get; set; }
    }