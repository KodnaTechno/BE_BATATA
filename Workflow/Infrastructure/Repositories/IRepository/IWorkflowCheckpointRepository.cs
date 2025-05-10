using AppWorkflow.Core.Domain.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppWorkflow.Infrastructure.Repositories.IRepository
{
    public interface IWorkflowCheckpointRepository
    {
        Task<WorkflowCheckpoint> GetByIdAsync(Guid id);
        Task<IEnumerable<WorkflowCheckpoint>> GetCheckpointsForInstanceAsync(Guid instanceId);
        Task CreateAsync(WorkflowCheckpoint checkpoint);
        Task UpdateAsync(WorkflowCheckpoint checkpoint);
        Task DeleteAsync(Guid id);
    }
}
