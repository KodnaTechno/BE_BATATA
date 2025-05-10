namespace AppWorkflow.Core.Interfaces.Services;


using System.Text;

public interface IWorkflowVersionManager
    {
        Task<Workflow> CreateNewVersionAsync(Guid workflowId);
        Task<Workflow> GetLatestVersionAsync(Guid workflowId);
        Task<IEnumerable<Workflow>> GetVersionHistoryAsync(Guid workflowId);
        Task<bool> IsLatestVersionAsync(Guid workflowId, string version);
    }