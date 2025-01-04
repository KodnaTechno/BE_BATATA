using AppWorkflow.Core.Interfaces.Services;
using AppWorkflow.Infrastructure.Data.Context;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppWorkflow.Infrastructure.Services
{
    public class ApprovalTimeoutService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ApprovalTimeoutService> _logger;

        public ApprovalTimeoutService(
            IServiceProvider serviceProvider,
            ILogger<ApprovalTimeoutService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessExpiredApprovalsAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing expired approvals");
                }

                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }

        private async Task ProcessExpiredApprovalsAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<WorkflowDbContext>();
            var workflowEngine = scope.ServiceProvider.GetRequiredService<IWorkflowEngine>();

            var expiredApprovals = await dbContext.ApprovalRequests
                .Where(a => a.Status == ApprovalStatus.Pending &&
                           a.ExpiresAt < DateTime.UtcNow)
                .ToListAsync();

            foreach (var approval in expiredApprovals)
            {
                approval.Status = ApprovalStatus.Expired;
                approval.UpdatedAt = DateTime.UtcNow;

                // Handle workflow timeout
                try
                {
                    await workflowEngine.HandleApprovalTimeoutAsync(
                        approval.WorkflowId,
                        approval.StepId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "Error handling approval timeout for workflow {WorkflowId}",
                        approval.WorkflowId);
                }
            }

            await dbContext.SaveChangesAsync();
        }
    }
}
