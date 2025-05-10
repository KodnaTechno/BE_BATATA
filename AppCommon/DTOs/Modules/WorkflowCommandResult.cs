// WARNING: This class name conflicts with Application.Features.WorkFlow.Command.WorkflowCommandResult
// If you intend to use only one, consider removing or renaming the other.
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
