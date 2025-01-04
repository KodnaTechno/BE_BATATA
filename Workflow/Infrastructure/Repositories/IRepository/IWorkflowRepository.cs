using AppWorkflow.Core.Domain.Data;
using AppWorkflow.Core.Domain.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppWorkflow.Infrastructure.Repositories.IRepository
{
    public interface IWorkflowRepository
    {
        Task<Workflow> GetByIdAsync(Guid id);
        Task<IEnumerable<Workflow>> GetWorkflowsByTriggerTypeAsync(string triggerType, string moduleType);
        Task<Workflow> GetLatestVersionAsync(Guid workflowId);
        Task<Workflow> GetWorkflowVersionAsync(Guid workflowId, string version);
        Task<IEnumerable<Workflow>> GetVersionHistoryAsync(Guid workflowId);
        Task<WorkflowData> GetInstanceAsync(Guid instanceId);
        Task CreateAsync(Workflow workflow);
        Task UpdateAsync(Workflow workflow);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
    }
}
