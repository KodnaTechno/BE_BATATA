using AppWorkflow.Core.Domain.Data;
using AppWorkflow.Services.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppWorkflow.Infrastructure.Services.Advance
{
    public class SynchronizationManager
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger<SynchronizationManager> _logger;
        private readonly IDistributedLockManager _lockManager;

        public async Task<bool> RegisterStepCompletionAsync(
            Guid syncPointId,
            Guid stepId)
        {
            var lockKey = $"sync-point-{syncPointId}";
            using var lockScope = await _lockManager.AcquireLockAsync(lockKey, TimeSpan.FromSeconds(30));

            var syncPoint = await GetSynchronizationPointAsync(syncPointId);
            if (syncPoint == null)
                return false;

            if (!syncPoint.CompletedStepIds.Contains(stepId))
            {
                syncPoint.CompletedStepIds.Add(stepId);
                if (syncPoint.IsMet)
                {
                    syncPoint.CompletedAt = DateTime.UtcNow;
                }
                await SaveSynchronizationPointAsync(syncPoint);
            }

            return syncPoint.IsMet;
        }

        public async Task<SynchronizationPoint> CreateSynchronizationPointAsync(
            Guid workflowInstanceId,
            IEnumerable<Guid> requiredStepIds,
            string name)
        {
            var syncPoint = new SynchronizationPoint
            {
                Id = Guid.NewGuid(),
                WorkflowInstanceId = workflowInstanceId,
                RequiredStepIds = requiredStepIds.ToList(),
                Name = name
            };

            await SaveSynchronizationPointAsync(syncPoint);
            return syncPoint;
        }

        private async Task<SynchronizationPoint> GetSynchronizationPointAsync(Guid syncPointId)
        {
            var cacheKey = $"sync-point-{syncPointId}";
            return new(); /*await _cache.GetAsync<SynchronizationPoint>(cacheKey);*/
        }

        private async Task SaveSynchronizationPointAsync(SynchronizationPoint syncPoint)
        {
            var cacheKey = $"sync-point-{syncPoint.Id}";
            //await _cache.SetAsync(cacheKey, syncPoint, TimeSpan.FromHours(1));
        }
    }
}
