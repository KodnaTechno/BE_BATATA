using AppWorkflow.Common.Enums;
using AppWorkflow.Core.Domain.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppWorkflow.Infrastructure.Repositories.IRepository
{
    public interface IWorkflowDataRepository
    {
        Task<WorkflowData> GetByIdAsync(Guid id);
        Task<IEnumerable<WorkflowData>> GetActiveInstancesAsync();
        Task<IEnumerable<WorkflowData>> GetInstancesByVersionAsync(string version);
        Task CreateAsync(WorkflowData instance);
        Task UpdateAsync(WorkflowData instance);
        Task UpdateInstanceAsync(WorkflowData instance);
        Task<IEnumerable<WorkflowData>> GetInstancesByStatus(WorkflowStatus status);
        Task<IEnumerable<WorkflowData>> GetInstancesByDateRange(DateTime start, DateTime end);
        ///
        Task<IEnumerable<WorkflowRelation>> GetWorkflowRelationsAsync(Guid instanceId);
        Task<IEnumerable<WorkflowData>> GetChildWorkflowsAsync(Guid parentInstanceId);
        Task<WorkflowData> GetParentWorkflowAsync(Guid childInstanceId);
        Task AddWorkflowRelationAsync(WorkflowRelation relation);
        Task UpdateWorkflowRelationAsync(WorkflowRelation relation);
        Task DeleteWorkflowRelationAsync(Guid relationId);
        Task<bool> HasActiveSubWorkflowsAsync(Guid parentInstanceId);
    }
}
