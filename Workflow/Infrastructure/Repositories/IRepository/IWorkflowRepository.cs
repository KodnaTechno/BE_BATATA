﻿using AppWorkflow.Core.Domain.Data;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AppWorkflow.Infrastructure.Repositories.IRepository
{    public interface IWorkflowRepository
    {
        Task<Workflow> GetByIdAsync(Guid id);
        Task<IEnumerable<Workflow>> GetWorkflowsByTriggerTypeAsync(string triggerType, string moduleType);
        Task<Workflow> GetLatestVersionAsync(Guid workflowId);
        Task<Workflow> GetWorkflowVersionAsync(Guid workflowId, string version);
        Task<IEnumerable<Workflow>> GetVersionHistoryAsync(Guid workflowId);
        Task<WorkflowData> GetInstanceAsync(Guid instanceId);
        Task CreateAsync(Workflow workflow,CancellationToken cancellationToken);
        Task UpdateAsync(Workflow workflow);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<Workflow> FindByActionIdAsync(Guid id, CancellationToken cancellationToken);
        Task<int> GetCountAsync();
    }
}
