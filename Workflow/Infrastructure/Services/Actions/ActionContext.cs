using AppWorkflow.Core.Models;
using System.Text.Json;

namespace AppWorkflow.Infrastructure.Services.Actions;

public class ActionContext
    {
        public Guid WorkflowDataId { get; set; }
        public Guid StepId { get; set; }
        public WorkflowModuleData ModuleData { get; set; }
        public JsonDocument ActionConfiguration { get; set; }
        public Dictionary<string, object> Variables { get; set; }
        public IServiceProvider ServiceProvider { get; set; }
        public CancellationToken CancellationToken { get; set; }
    }