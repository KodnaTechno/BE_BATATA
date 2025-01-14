using AppWorkflow.Core.Domain.Data;
using AppWorkflow.Core.Interfaces.Services;
using AppWorkflow.Infrastructure.Data.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppWorkflow.Infrastructure.Services
{
    public class ApprovalResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public Dictionary<string, object> UpdatedProperties { get; set; }
    }

    public interface IApprovalService
    {
        Task<Guid> CreateApprovalRequestAsync(ApprovalRequest request);
        Task<ApprovalRequest> GetApprovalRequestAsync(Guid approvalId);
        Task<ApprovalResult> ProcessApprovalAsync(Guid approvalId, bool approved, string userId, Dictionary<string, object> updatedProperties, string comments);
        Task<List<ApprovalRequest>> GetPendingApprovalsAsync(string userId);
        Task<List<string>> GetUsersByRoleAsync(string roleId);
        Task<List<string>> GetUsersByGroupAsync(string groupId);
    }
    public class ApprovalService : IApprovalService
    {
        private readonly WorkflowDbContext _dbContext;
        private readonly ILogger<ApprovalService> _logger;
        private readonly IWorkflowEngine _workflowEngine;
       // private readonly IIdentityService _identityService;

        public ApprovalService(
            WorkflowDbContext dbContext,
            ILogger<ApprovalService> logger,
            IWorkflowEngine workflowEngine 
            /*IIdentityService identityService*/)
        {
            _dbContext = dbContext;
            _logger = logger;
            _workflowEngine = workflowEngine;
            //_identityService = identityService;
        }

        public async Task<Guid> CreateApprovalRequestAsync(ApprovalRequest request)
        {
            try
            {
                request.Id = Guid.NewGuid();
                request.CreatedAt = DateTime.UtcNow;
                request.Status = ApprovalStatus.Pending;

                await _dbContext.ApprovalRequests.AddAsync(request);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation(
                    "Created approval request {ApprovalId} for workflow {WorkflowId}",
                    request.Id, request.WorkflowDataId);

                return request.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating approval request");
                throw;
            }
        }

        public async Task<ApprovalRequest> GetApprovalRequestAsync(Guid approvalId)
        {
            return await _dbContext.ApprovalRequests
                .FirstOrDefaultAsync(a => a.Id == approvalId);
        }

        public async Task<ApprovalResult> ProcessApprovalAsync(
            Guid approvalId,
            bool approved,
            string userId,
            Dictionary<string, object> updatedProperties,
            string comments)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var request = await GetApprovalRequestAsync(approvalId);
                if (request == null)
                    throw new InvalidOperationException($"Approval request {approvalId} not found");

                if (request.Status != ApprovalStatus.Pending)
                    throw new InvalidOperationException($"Approval request {approvalId} is not pending");

                if (!request.ApproverIds.Contains(userId))
                    throw new UnauthorizedAccessException($"User {userId} is not authorized to approve this request");

                // Validate updated properties
                if (updatedProperties != null)
                {
                    foreach (var prop in request.EditableProperties.Where(p => p.IsRequired))
                    {
                        if (!updatedProperties.ContainsKey(prop.PropertyName))
                            throw new InvalidOperationException($"Required property {prop.PropertyName} is missing");
                    }
                }

                // Update request status
                request.Status = approved ? ApprovalStatus.Approved : ApprovalStatus.Rejected;
                request.ProcessedBy = userId;
                request.ProcessedAt = DateTime.UtcNow;
                request.Comments = comments;
                request.UpdatedProperties = updatedProperties;

                await _dbContext.SaveChangesAsync();

                // Resume workflow
                if (approved)
                {
                    var workflowInstance = await _workflowEngine.GetInstanceAsync(request.WorkflowDataId);
                    if (workflowInstance != null)
                    {
                        // Update module properties if any
                        if (updatedProperties?.Any() == true)
                        {
                            // Update module properties logic here
                        }

                        await _workflowEngine.ResumeWorkflowAsync(request.WorkflowDataId);
                    }
                }
                else
                {
                    // Handle rejection - might need to transition to a different state
                    // or terminate the workflow based on configuration
                }

                await transaction.CommitAsync();

                return new ApprovalResult
                {
                    Success = true,
                    Message = approved ? "Request approved" : "Request rejected",
                    UpdatedProperties = updatedProperties
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error processing approval request {ApprovalId}", approvalId);
                throw;
            }
        }

        public async Task<List<ApprovalRequest>> GetPendingApprovalsAsync(string userId)
        {
            return await _dbContext.ApprovalRequests
                .Where(a => a.Status == ApprovalStatus.Pending &&
                           a.ApproverIds.Contains(userId) &&
                           a.ExpiresAt > DateTime.UtcNow)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<string>> GetUsersByRoleAsync(string roleId)
        {
            //return await _identityService.GetUsersByRoleAsync(roleId);
            return new List<string>();
        }

        public async Task<List<string>> GetUsersByGroupAsync(string groupId)
        {
            //return await _identityService.GetUsersByGroupAsync(groupId);
            return new List<string>();
        }
    }
}
