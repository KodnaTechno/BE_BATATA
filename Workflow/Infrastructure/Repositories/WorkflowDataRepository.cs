using AppWorkflow.Common.Enums;
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
    public class WorkflowDataRepository : IWorkflowDataRepository
    {
        private readonly WorkflowDbContext _context;
        private readonly ILogger<WorkflowDataRepository> _logger;
        private readonly IDistributedCache _cache;

        public WorkflowDataRepository(
            WorkflowDbContext context,
            ILogger<WorkflowDataRepository> logger,
            IDistributedCache cache)
        {
            _context = context;
            _logger = logger;
            _cache = cache;
        }

        public async Task<WorkflowData> GetByIdAsync(Guid id)
        {
            var cacheKey = $"workflow-instance:{id}";

            // Try get from cache
            //var cachedInstance = await _cache.GetAsync<WorkflowData>(cacheKey);
            //if (cachedInstance != null)
            //    return cachedInstance;

            try
            {
                var instance = await _context.WorkflowData
                    .Include(w => w.StepInstances)
                    .FirstOrDefaultAsync(w => w.Id == id && !w.IsDeleted);

                //if (instance != null)
                //{
                //    await _cache.SetAsync(cacheKey, instance, TimeSpan.FromMinutes(5));
                //}

                return instance;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving workflow instance {InstanceId}", id);
                throw new RepositoryException($"Error retrieving workflow instance {id}", ex);
            }
        }

        public async Task<IEnumerable<WorkflowData>> GetActiveInstancesAsync()
        {
            try
            {
                return await _context.WorkflowData
                    .Include(w => w.StepInstances)
                    .Where(w => w.Status == WorkflowStatus.Active && !w.IsDeleted)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving active workflow instances");
                throw new RepositoryException("Error retrieving active workflow instances", ex);
            }
        }

        public async Task<IEnumerable<WorkflowData>> GetInstancesByVersionAsync(string version)
        {
            try
            {
                return await _context.WorkflowData
                    .Include(w => w.StepInstances)
                    .Where(w => w.WorkflowVersion == version && !w.IsDeleted)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving instances for version {Version}", version);
                throw new RepositoryException($"Error retrieving instances for version {version}", ex);
            }
        }

        public async Task CreateAsync(WorkflowData instance)
        {
            try
            {
                await _context.WorkflowData.AddAsync(instance);
                await _context.SaveChangesAsync();

                // Cache the new instance
                var cacheKey = $"workflow-instance:{instance.Id}";
                //await _cache.SetAsync(cacheKey, instance, TimeSpan.FromMinutes(5));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating workflow instance {InstanceId}", instance.Id);
                throw new RepositoryException($"Error creating workflow instance {instance.Id}", ex);
            }
        }

        public async Task UpdateAsync(WorkflowData instance)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Entry(instance).State = EntityState.Modified;

                // Update step instances
                foreach (var stepInstance in instance.StepInstances)
                {
                    if (stepInstance.Id == Guid.Empty)
                    {
                        await _context.WorkflowStepData.AddAsync(stepInstance);
                    }
                    else
                    {
                        _context.Entry(stepInstance).State = EntityState.Modified;
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                // Update cache
                var cacheKey = $"workflow-instance:{instance.Id}";
                //await _cache.SetAsync(cacheKey, instance, TimeSpan.FromMinutes(5));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error updating workflow instance {InstanceId}", instance.Id);
                throw new RepositoryException($"Error updating workflow instance {instance.Id}", ex);
            }
        }

        public async Task UpdateInstanceAsync(WorkflowData instance)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var existing = await _context.WorkflowData
                    .Include(w => w.StepInstances)
                    .FirstOrDefaultAsync(w => w.Id == instance.Id);

                if (existing == null)
                    throw new WorkflowNotFoundException(instance.WorkflowId, instance.Id);

                // Update main instance properties
                _context.Entry(existing).CurrentValues.SetValues(instance);

                // Update step instances
                foreach (var stepInstance in instance.StepInstances)
                {
                    var existingStep = existing.StepInstances
                        .FirstOrDefault(s => s.Id == stepInstance.Id);

                    if (existingStep != null)
                    {
                        _context.Entry(existingStep).CurrentValues.SetValues(stepInstance);
                    }
                    else
                    {
                        existing.StepInstances.Add(stepInstance);
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                // Update cache
                var cacheKey = $"workflow-instance:{instance.Id}";
                //await _cache.SetAsync(cacheKey, instance, TimeSpan.FromMinutes(5));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error updating workflow instance {InstanceId}", instance.Id);
                throw new RepositoryException($"Error updating workflow instance {instance.Id}", ex);
            }
        }

        public async Task<IEnumerable<WorkflowData>> GetInstancesByStatus(WorkflowStatus status)
        {
            try
            {
                return await _context.WorkflowData
                    .Include(w => w.StepInstances)
                    .Where(w => w.Status == status && !w.IsDeleted)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving instances by status {Status}", status);
                throw new RepositoryException($"Error retrieving instances by status {status}", ex);
            }
        }
        public async Task<IEnumerable<WorkflowData>> GetInstancesByDateRange(DateTime start, DateTime end)
        {
            try
            {
                return await _context.WorkflowData
                    .Include(w => w.StepInstances)
                    .Where(w => w.CreatedAt >= start &&
                               w.CreatedAt <= end &&
                               !w.IsDeleted)
                    .OrderByDescending(w => w.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error retrieving instances between {Start} and {End}",
                    start, end);
                throw new RepositoryException(
                    $"Error retrieving instances for date range {start} to {end}",
                    ex);
            }
        }

        public async Task<IEnumerable<WorkflowStepData>> GetStepHistoryAsync(Guid instanceId)
        {
            try
            {
                return await _context.WorkflowStepData
                    .Where(s => s.WorkflowDataId == instanceId)
                    .OrderByDescending(s => s.StartedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error retrieving step history for instance {InstanceId}",
                    instanceId);
                throw new RepositoryException(
                    $"Error retrieving step history for instance {instanceId}",
                    ex);
            }
        }

        public async Task<IEnumerable<WorkflowData>> GetFailedInstancesAsync(DateTime since)
        {
            try
            {
                return await _context.WorkflowData
                    .Include(w => w.StepInstances)
                    .Where(w => w.Status == WorkflowStatus.Failed &&
                               w.UpdatedAt >= since &&
                               !w.IsDeleted)
                    .OrderByDescending(w => w.UpdatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error retrieving failed instances since {Since}",
                    since);
                throw new RepositoryException(
                    $"Error retrieving failed instances since {since}",
                    ex);
            }
        }

        public async Task<bool> HasActiveInstancesAsync(Guid workflowId)
        {
            try
            {
                return await _context.WorkflowData
                    .AnyAsync(w => w.WorkflowId == workflowId &&
                                  w.Status == WorkflowStatus.Active &&
                                  !w.IsDeleted);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error checking active instances for workflow {WorkflowId}",
                    workflowId);
                throw new RepositoryException(
                    $"Error checking active instances for workflow {workflowId}",
                    ex);
            }
        }

        public async Task<IDictionary<Guid, int>> GetActiveInstanceCountByWorkflowAsync(
            IEnumerable<Guid> workflowIds)
        {
            try
            {
                return await _context.WorkflowData
                    .Where(w => workflowIds.Contains(w.WorkflowId) &&
                               w.Status == WorkflowStatus.Active &&
                               !w.IsDeleted)
                    .GroupBy(w => w.WorkflowId)
                    .ToDictionaryAsync(
                        g => g.Key,
                        g => g.Count()
                    );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error getting active instance counts for workflows");
                throw new RepositoryException(
                    "Error getting active instance counts",
                    ex);
            }
        }

        public async Task BulkUpdateStatusAsync(
            IEnumerable<Guid> instanceIds,
            WorkflowStatus newStatus)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var instances = await _context.WorkflowData
                    .Where(w => instanceIds.Contains(w.Id))
                    .ToListAsync();

                foreach (var instance in instances)
                {
                    instance.Status = newStatus;
                    instance.UpdatedAt = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                // Update cache for each instance
                foreach (var instance in instances)
                {
                    var cacheKey = $"workflow-instance:{instance.Id}";
                    //await _cache.SetAsync(cacheKey, instance, TimeSpan.FromMinutes(5));
                }
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex,
                    "Error bulk updating status for {Count} instances",
                    instanceIds.Count());
                throw new RepositoryException(
                    "Error bulk updating instance statuses",
                    ex);
            }
        }

        public async Task CleanupStaleInstancesAsync(TimeSpan stalePeriod)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var cutoffDate = DateTime.UtcNow.Subtract(stalePeriod);

                var staleInstances = await _context.WorkflowData
                    .Where(w => w.UpdatedAt <= cutoffDate &&
                               (w.Status == WorkflowStatus.Completed ||
                                w.Status == WorkflowStatus.Failed ||
                                w.Status == WorkflowStatus.Terminated))
                    .ToListAsync();

                foreach (var instance in staleInstances)
                {
                    instance.IsDeleted = true;
                    instance.UpdatedAt = DateTime.UtcNow;

                    // Remove from cache
                    var cacheKey = $"workflow-instance:{instance.Id}";
                    await _cache.RemoveAsync(cacheKey);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation(
                    "Cleaned up {Count} stale workflow instances",
                    staleInstances.Count);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error cleaning up stale instances");
                throw new RepositoryException(
                    "Error cleaning up stale instances",
                    ex);
            }
        }
        public async Task<IEnumerable<WorkflowRelation>> GetWorkflowRelationsAsync(Guid instanceId)
        {
            try
            {
                return await _context.Set<WorkflowRelation>()
                    .Include(r => r.ParentInstance)
                    .Include(r => r.ChildInstance)
                    .Where(r => r.ParentInstanceId == instanceId || r.ChildInstanceId == instanceId)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving workflow relations for instance {InstanceId}", instanceId);
                throw new RepositoryException($"Error retrieving workflow relations for instance {instanceId}", ex);
            }
        }
        public async Task<IEnumerable<WorkflowData>> GetChildWorkflowsAsync(Guid parentInstanceId)
        {
            try
            {
                return await _context.WorkflowData
                    .Include(w => w.StepInstances)
                    .Where(w => w.ParentRelations.Any(r => r.ParentInstanceId == parentInstanceId))
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving child workflows for parent {ParentId}", parentInstanceId);
                throw new RepositoryException($"Error retrieving child workflows for parent {parentInstanceId}", ex);
            }
        }

        public async Task<WorkflowData> GetParentWorkflowAsync(Guid childInstanceId)
        {
            try
            {
                return await _context.WorkflowData
                    .Include(w => w.StepInstances)
                    .FirstOrDefaultAsync(w => w.ChildRelations.Any(r => r.ChildInstanceId == childInstanceId));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving parent workflow for child {ChildId}", childInstanceId);
                throw new RepositoryException($"Error retrieving parent workflow for child {childInstanceId}", ex);
            }
        }

        public async Task AddWorkflowRelationAsync(WorkflowRelation relation)
        {
            try
            {
                await _context.Set<WorkflowRelation>().AddAsync(relation);
                await _context.SaveChangesAsync();

                // Invalidate relevant cache entries
                await InvalidateRelationCacheAsync(relation.ParentInstanceId);
                await InvalidateRelationCacheAsync(relation.ChildInstanceId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding workflow relation between parent {ParentId} and child {ChildId}",
                    relation.ParentInstanceId, relation.ChildInstanceId);
                throw new RepositoryException("Error adding workflow relation", ex);
            }
        }

        public async Task UpdateWorkflowRelationAsync(WorkflowRelation relation)
        {
            try
            {
                _context.Set<WorkflowRelation>().Update(relation);
                await _context.SaveChangesAsync();

                // Invalidate relevant cache entries
                await InvalidateRelationCacheAsync(relation.ParentInstanceId);
                await InvalidateRelationCacheAsync(relation.ChildInstanceId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating workflow relation {RelationId}", relation.Id);
                throw new RepositoryException($"Error updating workflow relation {relation.Id}", ex);
            }
        }

        public async Task DeleteWorkflowRelationAsync(Guid relationId)
        {
            try
            {
                var relation = await _context.Set<WorkflowRelation>().FindAsync(relationId);
                if (relation != null)
                {
                    _context.Set<WorkflowRelation>().Remove(relation);
                    await _context.SaveChangesAsync();

                    // Invalidate relevant cache entries
                    await InvalidateRelationCacheAsync(relation.ParentInstanceId);
                    await InvalidateRelationCacheAsync(relation.ChildInstanceId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting workflow relation {RelationId}", relationId);
                throw new RepositoryException($"Error deleting workflow relation {relationId}", ex);
            }
        }

        public async Task<bool> HasActiveSubWorkflowsAsync(Guid parentInstanceId)
        {
            try
            {
                return await _context.Set<WorkflowRelation>()
                    .Include(r => r.ChildInstance)
                    .AnyAsync(r => r.ParentInstanceId == parentInstanceId &&
                                  r.RelationType == WorkflowRelationType.SubWorkflow &&
                                  r.ChildInstance.Status == WorkflowStatus.Active);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking active sub-workflows for parent {ParentId}", parentInstanceId);
                throw new RepositoryException($"Error checking active sub-workflows for parent {parentInstanceId}", ex);
            }
        }

        private async Task InvalidateRelationCacheAsync(Guid instanceId)
        {
            var cacheKey = $"workflow-instance:{instanceId}";
            await _cache.RemoveAsync(cacheKey);
        }
    }
}
