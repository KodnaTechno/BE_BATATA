using System.Text.Json;

namespace AppWorkflow.Infrastructure.Data.Configurations;

public class TriggerConfiguration
    {
        public string Type { get; set; }
        public JsonDocument Configuration { get; set; }
        public Dictionary<string, string> Metadata { get; set; }
    }