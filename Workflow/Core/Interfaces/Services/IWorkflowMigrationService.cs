namespace AppWorkflow.Core.Interfaces.Services;

using AppWorkflow.Core.Domain.Data;
using System.Text;

public interface IWorkflowMigrationService
    {
        Task<bool> MigrateInstanceAsync(Guid instanceId, string targetVersion);
        Task<IEnumerable<WorkflowData>> GetInstancesForVersionAsync(string version);
        Task ValidateMigrationPathAsync(string sourceVersion, string targetVersion);
    }