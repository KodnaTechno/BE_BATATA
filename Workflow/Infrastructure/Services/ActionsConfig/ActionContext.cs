using AppWorkflow.Core.Models;
using AppWorkflow.Infrastructure.Data.Context;
using System.Text.Json;

namespace AppWorkflow.Infrastructure.Services.Actions;

public class ActionContext
    {
        public Guid WorkflowDataId { get; set; }
        public Guid StepId { get; set; }
        public WorkflowModuleData? ModuleData { get; set; }
        public JsonDocument? ActionConfiguration { get; set; }
        public Dictionary<string, object>? Variables { get; set; }
        public IServiceProvider? ServiceProvider { get; set; }
        public CancellationToken CancellationToken { get; set; }
        public Guid? CurrentUserId { get; set; }
        public WorkflowExecutionContext? WorkflowExecutionContext  { get;set;}

        // Strongly-typed config helper
        public TConfig? GetConfig<TConfig>() where TConfig : class, new()
        {
            if (ActionConfiguration == null)
                return new TConfig();
            return JsonSerializer.Deserialize<TConfig>(ActionConfiguration.RootElement.GetRawText());
        }

        // Safe variable getter
        public T? GetVariable<T>(string key, T? defaultValue = default)
        {
            if (Variables != null && Variables.TryGetValue(key, out var value) && value is T tValue)
                return tValue;
            return defaultValue;
        }
    }