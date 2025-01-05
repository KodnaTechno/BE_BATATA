namespace AppWorkflow.Infrastructure.Services.Actions;

public class ActionResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public Dictionary<string, object> OutputVariables { get; set; } = new();
        public object ResultData { get; set; }
        public List<string> Warnings { get; set; } = new();
        public Exception Exception { get; set; }
    }