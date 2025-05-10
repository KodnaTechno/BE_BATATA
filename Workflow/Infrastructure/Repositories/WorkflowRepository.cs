using AppWorkflow.Common.Exceptions;
using AppWorkflow.Core.Domain.Data;

using AppWorkflow.Infrastructure.Data.Context;
using AppWorkflow.Infrastructure.Repositories.IRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppWorkflow.Infrastructure.Repositories
{
    public class WorkflowRepository : IWorkflowRepository
    {
        private readonly WorkflowDbContext _context;
        private readonly ILogger<WorkflowRepository> _logger;
        private readonly IDistributedCache _cache;

        public WorkflowRepository(
            WorkflowDbContext context,
            ILogger<WorkflowRepository> logger,
            IDistributedCache cache)
        {
            _context = context;
            _logger = logger;
            _cache = cache;
        }

        public async Task<Workflow> GetByIdAsync(Guid id)
        {
            var cacheKey = $"workflow:{id}";

            // Try get from cache
            //var cachedWorkflow = await _cache.GetAsync<Workflow>(cacheKey);
            //if (cachedWorkflow != null)
            //    return cachedWorkflow;

            try
            {
                var workflow = await _context.Workflows
                    .Include(w => w.Steps)
                        .ThenInclude(s => s.Transitions)
              
                    .FirstOrDefaultAsync(w => w.Id == id && !w.IsDeleted);

                if (workflow != null)
                {
                  //  await _cache.SetAsync(cacheKey, workflow, TimeSpan.FromMinutes(30));
                }

                return workflow;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving workflow {WorkflowId}", id);
                throw new RepositoryException($"Error retrieving workflow {id}", ex);
            }
        }

        public async Task<IEnumerable<Workflow>> GetWorkflowsByTriggerTypeAsync(string triggerType, string moduleType)
        {
            try
            {
                return await _context.Workflows
                    .Include(w => w.Steps)
                    .Include(w => w.Variables)
                    .Where(w =>
                        w.IsLatestVersion &&
                        !w.IsDeleted &&
                        (w.TriggerConfigs != null && w.TriggerConfigs.Any(c => c.Type == triggerType)) &&
                        (string.IsNullOrEmpty(moduleType) ? string.IsNullOrEmpty(w.ModuleType) : w.ModuleType == moduleType)
                    )
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving workflows for trigger {TriggerType}", triggerType);
                throw new RepositoryException($"Error retrieving workflows for trigger {triggerType}", ex);
            }
        }

        public async Task<Workflow> GetLatestVersionAsync(Guid workflowId)
        {
            try
            {
                return await _context.Workflows
                    .Include(w => w.Steps)
                        .ThenInclude(s => s.Transitions)
                    .Include(w => w.Variables)
                    .FirstOrDefaultAsync(w => w.Id == workflowId &&
                                            w.IsLatestVersion &&
                                            !w.IsDeleted);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving latest version for workflow {WorkflowId}", workflowId);
                throw new RepositoryException($"Error retrieving latest version for workflow {workflowId}", ex);
            }
        }

        public async Task<Workflow> GetWorkflowVersionAsync(Guid workflowId, string version)
        {
            try
            {
                return await _context.Workflows
                    .Include(w => w.Steps)
                        .ThenInclude(s => s.Transitions)
                    .Include(w => w.Variables)
                    .FirstOrDefaultAsync(w => w.Id == workflowId &&
                                            w.Version == version &&
                                            !w.IsDeleted);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving version {Version} for workflow {WorkflowId}",
                    version, workflowId);
                throw new RepositoryException($"Error retrieving version {version} for workflow {workflowId}", ex);
            }
        }

        public async Task<IEnumerable<Workflow>> GetVersionHistoryAsync(Guid workflowId)
        {
            try
            {
                return await _context.Workflows
                    .Where(w => w.Id == workflowId && !w.IsDeleted)
                    .OrderByDescending(w => w.Version)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving version history for workflow {WorkflowId}", workflowId);
                throw new RepositoryException($"Error retrieving version history for workflow {workflowId}", ex);
            }
        }

        public async Task CreateAsync(Workflow workflow, CancellationToken cancellationToken)
        {
            try
            {
                await _context.Workflows.AddAsync(workflow);
                await _context.SaveChangesAsync();

                // Cache the new workflow
                var cacheKey = $"workflow:{workflow.Id}";
               // await _cache.SetAsync(cacheKey, workflow, TimeSpan.FromMinutes(30));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating workflow {WorkflowId}", workflow.Id);
                throw new RepositoryException($"Error creating workflow {workflow.Id}", ex);
            }
        }

        public async Task UpdateAsync(Workflow workflow)
        {
            try
            {
                _context.Entry(workflow).State = EntityState.Modified;
                foreach (var step in workflow.Steps)
                {
                    _context.Entry(step).State = EntityState.Modified;
                }
                await _context.SaveChangesAsync();

                // Update cache
                var cacheKey = $"workflow:{workflow.Id}";
                //await _cache.SetAsync(cacheKey, workflow, TimeSpan.FromMinutes(30));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating workflow {WorkflowId}", workflow.Id);
                throw new RepositoryException($"Error updating workflow {workflow.Id}", ex);
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            try
            {
                var workflow = await _context.Workflows.FindAsync(id);
                if (workflow != null)
                {
                    workflow.IsDeleted = true;
                    workflow.UpdatedAt = DateTime.UtcNow;
                    await _context.SaveChangesAsync();

                    // Remove from cache
                    var cacheKey = $"workflow:{id}";
                    await _cache.RemoveAsync(cacheKey);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting workflow {WorkflowId}", id);
                throw new RepositoryException($"Error deleting workflow {id}", ex);
            }
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            try
            {
                return await _context.Workflows
                    .AnyAsync(w => w.Id == id && !w.IsDeleted);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking existence of workflow {WorkflowId}", id);
                throw new RepositoryException($"Error checking existence of workflow {id}", ex);
            }
        }

        public Task<WorkflowData> GetInstanceAsync(Guid instanceId)
        {
            throw new NotImplementedException();
        }

        public async Task<Workflow> FindByActionIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return  _context.Workflows.ToList()
                        .Where(c =>
                            c.Metadata.Any(m => m.Key == "ModuleId" && m.Value.ToLower() == id.ToString().ToLower())
                            && c.Metadata.Any(m => m.Key == "ModuleType" && m.Value == "AppAction")
                        )
                        .SingleOrDefault();
        }
    }
}
