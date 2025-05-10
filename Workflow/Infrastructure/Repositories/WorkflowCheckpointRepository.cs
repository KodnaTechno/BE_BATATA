using AppWorkflow.Core.Domain.Data;
using AppWorkflow.Infrastructure.Data.Context;
using AppWorkflow.Infrastructure.Repositories.IRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppWorkflow.Infrastructure.Repositories
{
    public class WorkflowCheckpointRepository : IWorkflowCheckpointRepository
    {
        private readonly WorkflowDbContext _dbContext;
        private readonly ILogger<WorkflowCheckpointRepository> _logger;

        public WorkflowCheckpointRepository(WorkflowDbContext dbContext, ILogger<WorkflowCheckpointRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<WorkflowCheckpoint> GetByIdAsync(Guid id)
        {
            try
            {
                return await _dbContext.WorkflowCheckpoints.FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving checkpoint with ID {CheckpointId}", id);
                throw;
            }
        }

        public async Task<IEnumerable<WorkflowCheckpoint>> GetCheckpointsForInstanceAsync(Guid instanceId)
        {
            try
            {
                return await _dbContext.WorkflowCheckpoints
                    .Where(c => c.InstanceId == instanceId)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving checkpoints for workflow instance {InstanceId}", instanceId);
                throw;
            }
        }

        public async Task CreateAsync(WorkflowCheckpoint checkpoint)
        {
            try
            {
                await _dbContext.WorkflowCheckpoints.AddAsync(checkpoint);
                await _dbContext.SaveChangesAsync();
                _logger.LogInformation("Created checkpoint {CheckpointId} for workflow {InstanceId}", checkpoint.Id, checkpoint.InstanceId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating checkpoint for workflow instance {InstanceId}", checkpoint.InstanceId);
                throw;
            }
        }

        public async Task UpdateAsync(WorkflowCheckpoint checkpoint)
        {
            try
            {
                _dbContext.WorkflowCheckpoints.Update(checkpoint);
                await _dbContext.SaveChangesAsync();
                _logger.LogInformation("Updated checkpoint {CheckpointId}", checkpoint.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating checkpoint {CheckpointId}", checkpoint.Id);
                throw;
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            try
            {
                var checkpoint = await _dbContext.WorkflowCheckpoints.FindAsync(id);
                if (checkpoint != null)
                {
                    _dbContext.WorkflowCheckpoints.Remove(checkpoint);
                    await _dbContext.SaveChangesAsync();
                    _logger.LogInformation("Deleted checkpoint {CheckpointId}", id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting checkpoint {CheckpointId}", id);
                throw;
            }
        }
    }
}
