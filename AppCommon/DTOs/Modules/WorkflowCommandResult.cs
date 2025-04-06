namespace AppCommon.DTOs.Modules
{
    public class WorkflowCommandResult
    {
        public bool Success { get; set; }
        public object Result { get; set; }
        public Dictionary<string, object> OutputVariables { get; set; }
        public string Message { get; set; }
    }
}
