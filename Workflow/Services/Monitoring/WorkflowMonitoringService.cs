using AppCommon.DTOs;
using AppWorkflow.Common.Enums;
using AppWorkflow.Core.Domain.Data;
using AppWorkflow.Engine;
using AppWorkflow.Infrastructure.Repositories.IRepository;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppWorkflow.Services.Monitoring
{
    public interface IWorkflowMonitoringService
    {
        Task<WorkflowMonitoringSummary> GetDashboardSummaryAsync();
        Task<WorkflowInstanceDetails> GetWorkflowInstanceDetailsAsync(Guid instanceId);
        Task<IEnumerable<WorkflowInstanceSummary>> GetActiveWorkflowInstancesAsync(int maxResults = 100);
        Task<IEnumerable<WorkflowInstanceSummary>> GetRecentWorkflowInstancesAsync(int maxResults = 100);
        Task<IEnumerable<WorkflowInstanceSummary>> GetFailedWorkflowInstancesAsync(int maxResults = 100);
        Task<WorkflowPerformanceMetrics> GetPerformanceMetricsAsync(Guid? workflowId = null);
        Task<IEnumerable<WorkflowInstanceSummary>> SearchWorkflowInstancesAsync(WorkflowSearchCriteria criteria);
    }

    public class WorkflowMonitoringService : IWorkflowMonitoringService
    {
        private readonly IWorkflowDataRepository _instanceRepository;
        private readonly IWorkflowRepository _workflowRepository;
        private readonly IWorkflowCheckpointRepository _checkpointRepository;
        private readonly ITelemetryTracker _telemetry;
        private readonly ILogger<WorkflowMonitoringService> _logger;

        public WorkflowMonitoringService(
            IWorkflowDataRepository instanceRepository,
            IWorkflowRepository workflowRepository,
            IWorkflowCheckpointRepository checkpointRepository,
            ITelemetryTracker telemetry,
            ILogger<WorkflowMonitoringService> logger)
        {
            _instanceRepository = instanceRepository;
            _workflowRepository = workflowRepository;
            _checkpointRepository = checkpointRepository;
            _telemetry = telemetry;
            _logger = logger;
        }

        public async Task<WorkflowMonitoringSummary> GetDashboardSummaryAsync()
        {
            try
            {
                // Gather statistics for workflows
                var instances = await _instanceRepository.GetAllInstancesAsync();
                
                var summary = new WorkflowMonitoringSummary
                {
                    TotalWorkflows = await _workflowRepository.GetCountAsync(),
                    TotalInstances = instances.Count(),
                    ActiveInstances = instances.Count(i => i.Status == WorkflowStatus.Active),
                    CompletedInstances = instances.Count(i => i.Status == WorkflowStatus.Completed),
                    FailedInstances = instances.Count(i => i.Status == WorkflowStatus.Failed ),
                    SuspendedInstances = instances.Count(i => i.Status == WorkflowStatus.Suspended),
                    TerminatedInstances = instances.Count(i => i.Status == WorkflowStatus.Terminated),
                    InstancesByStatus = new Dictionary<string, int>
                    {
                        [WorkflowStatus.Active.ToString()] = instances.Count(i => i.Status == WorkflowStatus.Active),
                        [WorkflowStatus.Completed.ToString()] = instances.Count(i => i.Status == WorkflowStatus.Completed),
                        [WorkflowStatus.Failed.ToString()] = instances.Count(i => i.Status == WorkflowStatus.Failed),
                        [WorkflowStatus.Suspended.ToString()] = instances.Count(i => i.Status == WorkflowStatus.Suspended),
                        [WorkflowStatus.Terminated.ToString()] = instances.Count(i => i.Status == WorkflowStatus.Terminated),
                    },
                    InstancesToday = instances.Count(i => i.StartedAt.Date == DateTime.UtcNow.Date),
                    InstancesByType = instances
                        .GroupBy(i => i.ModuleData?.ModuleType ?? "Unknown")
                        .ToDictionary(g => g.Key, g => g.Count()),
                    AvgCompletionTime = instances
                        .Where(i => i.Status == WorkflowStatus.Completed && i.CompletedAt != null)
                        .Select(i => (i.CompletedAt.Value - i.StartedAt).TotalSeconds)
                        .DefaultIfEmpty(0)
                        .Average(),
                    RecentErrors = instances
                        .Where(i => (i.Status == WorkflowStatus.Failed) && 
                               i.UpdatedAt != null && 
                               i.UpdatedAt.Value > DateTime.UtcNow.AddDays(-1))
                        .Take(5)
                        .Select(i => new WorkflowError
                        {
                            InstanceId = i.Id,
                            WorkflowId = i.WorkflowId,
                            ModuleType = i.ModuleData?.ModuleType,
                            ErrorMessage = i.ErrorDetails,
                            Timestamp = i.UpdatedAt.Value
                        })
                        .ToList()
                };

                return summary;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating workflow monitoring summary");
                throw;
            }
        }

        public async Task<WorkflowInstanceDetails> GetWorkflowInstanceDetailsAsync(Guid instanceId)
        {
            try
            {
                var instance = await _instanceRepository.GetByIdAsync(instanceId);
                if (instance == null)
                {
                    return null;
                }

                var workflow = await _workflowRepository.GetByIdAsync(instance.WorkflowId);
                var checkpoints = await _checkpointRepository.GetCheckpointsForInstanceAsync(instanceId);

                var details = new WorkflowInstanceDetails
                {
                    InstanceId = instance.Id,
                    WorkflowId = instance.WorkflowId,
                    WorkflowName = workflow?.Name ?? "Unknown",
                    Status = instance.Status,
                    ModuleType = instance.ModuleData?.ModuleType,
                    StartedAt = instance.StartedAt,
                    CompletedAt = instance.CompletedAt,
                    CurrentStepId = instance.CurrentStepId,
                    Duration = instance.CompletedAt.HasValue 
                        ? instance.CompletedAt.Value - instance.StartedAt
                        : DateTime.UtcNow - instance.StartedAt,
                    ErrorDetails = instance.ErrorDetails,
                    Variables = instance.Variables,
                    StepHistory = instance.StepInstances?
                        .OrderBy(s => s.StartedAt)
                        .Select(s => new StepHistoryItem
                        {
                            StepId = s.StepId,
                            StepName = workflow?.Steps.FirstOrDefault(ws => ws.Id == s.StepId)?.Name ?? "Unknown",
                            Status = s.Status,
                            StartedAt = s.StartedAt,
                            CompletedAt = s.CompletedAt,
                            Duration = s.CompletedAt.HasValue
                                ? s.CompletedAt.Value - s.StartedAt
                                : TimeSpan.Zero,
                            InputData = s.InputData,
                            OutputData = s.OutputData,
                            ErrorDetails = s.ErrorDetails,
                            RetryCount = s.RetryCount
                        }).ToList() ?? new List<StepHistoryItem>(),
                    Checkpoints = checkpoints?
                        .OrderByDescending(c => c.CheckpointTime)
                        .Select(c => new CheckpointInfo
                        {
                            CheckpointId = c.Id,
                            CreatedAt = c.CheckpointTime,
                            StepId = c.CurrentStepId,
                            StepName = workflow?.Steps.FirstOrDefault(ws => ws.Id == c.CurrentStepId)?.Name ?? "Unknown",
                            IsAutomatic = c.IsAutomatic,
                            Description = c.Description
                        }).ToList() ?? new List<CheckpointInfo>()
                };

                return details;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving workflow instance details for {InstanceId}", instanceId);
                throw;
            }
        }

        public async Task<IEnumerable<WorkflowInstanceSummary>> GetActiveWorkflowInstancesAsync(int maxResults = 100)
        {
            var activeInstances = await _instanceRepository.GetActiveInstancesAsync(maxResults);
            return await ConvertToSummariesAsync(activeInstances);
        }

        public async Task<IEnumerable<WorkflowInstanceSummary>> GetRecentWorkflowInstancesAsync(int maxResults = 100)
        {
            var recentInstances = await _instanceRepository.GetRecentInstancesAsync(maxResults);
            return await ConvertToSummariesAsync(recentInstances);
        }

        public async Task<IEnumerable<WorkflowInstanceSummary>> GetFailedWorkflowInstancesAsync(int maxResults = 100)
        {
            var failedInstances = await _instanceRepository.GetFailedInstancesAsync(maxResults);
            return await ConvertToSummariesAsync(failedInstances);
        }

        public async Task<WorkflowPerformanceMetrics> GetPerformanceMetricsAsync(Guid? workflowId = null)
        {            try
            {
                // Get all completed instances within last 30 days, or specific workflow if provided
                var instances = await _instanceRepository.GetRecentCompletedInstancesAsync(
                    100, // Get up to 100 recent completions
                    30); // From the last 30 days
                
                // Filter by workflowId if provided
                if (workflowId.HasValue)
                {
                    instances = instances.Where(i => i.WorkflowId == workflowId.Value);
                }

                var metrics = new WorkflowPerformanceMetrics
                {
                    TotalExecutions = instances.Count(),
                    SuccessRate = CalculateSuccessRate(instances),
                    AverageExecutionTime = CalculateAverageExecutionTime(instances),
                    P95ExecutionTime = CalculatePercentileExecutionTime(instances, 95),
                    StepMetrics = await GetStepMetricsAsync(instances)
                };

                // Add time trend data by day
                var dateGroups = instances
                    .GroupBy(i => i.StartedAt.Date)
                    .OrderBy(g => g.Key);

                foreach (var group in dateGroups)
                {
                    var executions = group.Count();
                    var successes = group.Count(i => i.Status == WorkflowStatus.Completed);
                    var avgDuration = group
                        .Where(i => i.CompletedAt.HasValue)
                        .Select(i => (i.CompletedAt.Value - i.StartedAt).TotalSeconds)
                        .DefaultIfEmpty(0)
                        .Average();

                    metrics.DailyMetrics.Add(new DailyMetrics
                    {
                        Date = group.Key,
                        Executions = executions,
                        SuccessRate = executions > 0 ? (double)successes / executions * 100 : 0,
                        AverageExecutionTime = TimeSpan.FromSeconds(avgDuration)
                    });
                }

                return metrics;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating performance metrics");
                throw;
            }
        }        public async Task<IEnumerable<WorkflowInstanceSummary>> SearchWorkflowInstancesAsync(WorkflowSearchCriteria criteria)
        {
            // Convert ModuleType to a search keyword if provided
            string keyword = !string.IsNullOrEmpty(criteria.ModuleType) ? criteria.ModuleType : null;
            
            var instances = await _instanceRepository.SearchInstancesAsync(
                keyword,
                criteria.StartTimeFrom,
                criteria.StartTimeTo,
                criteria.Status,
                null, // version not specified in criteria
                100); // Limit to 100 results

            // If WorkflowId was specified, filter the results
            if (criteria.WorkflowId.HasValue)
            {
                instances = instances.Where(i => i.WorkflowId == criteria.WorkflowId.Value);
            }

            return await ConvertToSummariesAsync(instances);
        }

        private async Task<IEnumerable<WorkflowInstanceSummary>> ConvertToSummariesAsync(IEnumerable<WorkflowData> instances)
        {
            var result = new List<WorkflowInstanceSummary>();
            var workflowCache = new Dictionary<Guid, Workflow>();

            foreach (var instance in instances)
            {
                // Get workflow from cache or repository
                if (!workflowCache.TryGetValue(instance.WorkflowId, out var workflow))
                {
                    workflow = await _workflowRepository.GetByIdAsync(instance.WorkflowId);
                    if (workflow != null)
                    {
                        workflowCache[instance.WorkflowId] = workflow;
                    }
                }

                result.Add(new WorkflowInstanceSummary
                {
                    InstanceId = instance.Id,
                    WorkflowId = instance.WorkflowId,
                    WorkflowName = workflow?.Name ?? "Unknown",
                    Status = instance.Status,
                    ModuleType = instance.ModuleData?.ModuleType,
                    StartedAt = instance.StartedAt,
                    CompletedAt = instance.CompletedAt,
                    Duration = instance.CompletedAt.HasValue
                        ? instance.CompletedAt.Value - instance.StartedAt
                        : DateTime.UtcNow - instance.StartedAt,
                    HasError = !string.IsNullOrEmpty(instance.ErrorDetails),
                    CurrentStepName = workflow?.Steps
                        .FirstOrDefault(s => s.Id == instance.CurrentStepId)?.Name ?? "Unknown",
                    StepsCompleted = instance.StepInstances?.Count(s => s.Status == StepStatus.Completed) ?? 0,
                    TotalSteps = workflow?.Steps?.Count ?? 0
                });
            }

            return result;
        }

        private double CalculateSuccessRate(IEnumerable<WorkflowData> instances)
        {
            if (!instances.Any())
                return 0;

            var completedCount = instances.Count(i => i.Status == WorkflowStatus.Completed);
            return (double)completedCount / instances.Count() * 100;
        }

        private TimeSpan CalculateAverageExecutionTime(IEnumerable<WorkflowData> instances)
        {
            var completedInstances = instances
                .Where(i => i.Status == WorkflowStatus.Completed && i.CompletedAt.HasValue);
            
            if (!completedInstances.Any())
                return TimeSpan.Zero;
            
            var average = completedInstances
                .Select(i => (i.CompletedAt.Value - i.StartedAt).TotalSeconds)
                .Average();
            
            return TimeSpan.FromSeconds(average);
        }

        private TimeSpan CalculatePercentileExecutionTime(IEnumerable<WorkflowData> instances, int percentile)
        {
            var completedInstances = instances
                .Where(i => i.Status == WorkflowStatus.Completed && i.CompletedAt.HasValue)
                .Select(i => (i.CompletedAt.Value - i.StartedAt).TotalSeconds)
                .OrderBy(d => d)
                .ToList();
            
            if (!completedInstances.Any())
                return TimeSpan.Zero;
            
            var index = (int)Math.Ceiling(percentile / 100.0 * completedInstances.Count) - 1;
            index = Math.Max(0, Math.Min(completedInstances.Count - 1, index));
            
            return TimeSpan.FromSeconds(completedInstances[index]);
        }

        private async Task<List<StepMetrics>> GetStepMetricsAsync(IEnumerable<WorkflowData> instances)
        {
            var result = new Dictionary<Guid, StepMetrics>();
            var workflowCache = new Dictionary<Guid, Workflow>();

            foreach (var instance in instances)
            {
                // Get workflow from cache or repository
                if (!workflowCache.TryGetValue(instance.WorkflowId, out var workflow))
                {
                    workflow = await _workflowRepository.GetByIdAsync(instance.WorkflowId);
                    if (workflow != null)
                    {
                        workflowCache[instance.WorkflowId] = workflow;
                    }
                }

                if (workflow == null || instance.StepInstances == null)
                    continue;

                foreach (var step in instance.StepInstances)
                {
                    var stepDefinition = workflow.Steps.FirstOrDefault(s => s.Id == step.StepId);
                    if (stepDefinition == null)
                        continue;

                    if (!result.TryGetValue(step.StepId, out var metrics))
                    {
                        metrics = new StepMetrics
                        {
                            StepId = step.StepId,
                            StepName = stepDefinition.Name,
                            ActionType = stepDefinition.ActionType,
                            ExecutionCount = 0,
                            SuccessCount = 0,
                            FailureCount = 0,
                            RetryCount = 0,
                            TotalExecutionTime = TimeSpan.Zero
                        };
                        result[step.StepId] = metrics;
                    }

                    metrics.ExecutionCount++;
                    
                    if (step.Status == StepStatus.Completed)
                        metrics.SuccessCount++;
                    else if (step.Status == StepStatus.Failed)
                        metrics.FailureCount++;
                    
                    metrics.RetryCount += step.RetryCount;
                    
                    if (step.CompletedAt.HasValue)
                        metrics.TotalExecutionTime += step.CompletedAt.Value - step.StartedAt;
                }
            }

            // Calculate averages
            foreach (var metrics in result.Values)
            {
                metrics.SuccessRate = metrics.ExecutionCount > 0 
                    ? (double)metrics.SuccessCount / metrics.ExecutionCount * 100 
                    : 0;
                
                metrics.AverageExecutionTime = metrics.ExecutionCount > 0
                    ? TimeSpan.FromTicks(metrics.TotalExecutionTime.Ticks / metrics.ExecutionCount)
                    : TimeSpan.Zero;
            }

            return result.Values.ToList();
        }
    }

    #region DTO Classes

    public class WorkflowMonitoringSummary
    {
        public int TotalWorkflows { get; set; }
        public int TotalInstances { get; set; }
        public int ActiveInstances { get; set; }
        public int CompletedInstances { get; set; }
        public int FailedInstances { get; set; }
        public int SuspendedInstances { get; set; }
        public int TerminatedInstances { get; set; }
        public int InstancesToday { get; set; }
        public Dictionary<string, int> InstancesByStatus { get; set; } = new();
        public Dictionary<string, int> InstancesByType { get; set; } = new();
        public double AvgCompletionTime { get; set; }
        public List<WorkflowError> RecentErrors { get; set; } = new();
    }

    public class WorkflowInstanceSummary
    {
        public Guid InstanceId { get; set; }
        public Guid WorkflowId { get; set; }
        public string WorkflowName { get; set; }
        public WorkflowStatus Status { get; set; }
        public string ModuleType { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public TimeSpan Duration { get; set; }
        public bool HasError { get; set; }
        public string CurrentStepName { get; set; }
        public int StepsCompleted { get; set; }
        public int TotalSteps { get; set; }
    }

    public class WorkflowInstanceDetails
    {
        public Guid InstanceId { get; set; }
        public Guid WorkflowId { get; set; }
        public string WorkflowName { get; set; }
        public WorkflowStatus Status { get; set; }
        public string ModuleType { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public TimeSpan Duration { get; set; }
        public string ErrorDetails { get; set; }
        public Dictionary<string, object> Variables { get; set; }
        public Guid CurrentStepId { get; set; }
        public List<StepHistoryItem> StepHistory { get; set; } = new();
        public List<CheckpointInfo> Checkpoints { get; set; } = new();
    }

    public class StepHistoryItem
    {
        public Guid StepId { get; set; }
        public string StepName { get; set; }
        public StepStatus Status { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public TimeSpan Duration { get; set; }
        public Dictionary<string, object> InputData { get; set; }
        public Dictionary<string, object> OutputData { get; set; }
        public string ErrorDetails { get; set; }
        public int RetryCount { get; set; }
    }

    public class CheckpointInfo
    {
        public Guid CheckpointId { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid StepId { get; set; }
        public string StepName { get; set; }
        public bool IsAutomatic { get; set; }
        public string Description { get; set; }
    }

    public class WorkflowError
    {
        public Guid InstanceId { get; set; }
        public Guid WorkflowId { get; set; }
        public string ModuleType { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class WorkflowPerformanceMetrics
    {
        public int TotalExecutions { get; set; }
        public double SuccessRate { get; set; }
        public TimeSpan AverageExecutionTime { get; set; }
        public TimeSpan P95ExecutionTime { get; set; }
        public List<StepMetrics> StepMetrics { get; set; } = new();
        public List<DailyMetrics> DailyMetrics { get; set; } = new();
    }

    public class StepMetrics
    {
        public Guid StepId { get; set; }
        public string StepName { get; set; }
        public string ActionType { get; set; }
        public int ExecutionCount { get; set; }
        public int SuccessCount { get; set; }
        public int FailureCount { get; set; }
        public int RetryCount { get; set; }
        public double SuccessRate { get; set; }
        public TimeSpan TotalExecutionTime { get; set; }
        public TimeSpan AverageExecutionTime { get; set; }
    }

    public class DailyMetrics
    {
        public DateTime Date { get; set; }
        public int Executions { get; set; }
        public double SuccessRate { get; set; }
        public TimeSpan AverageExecutionTime { get; set; }
    }

    public class WorkflowSearchCriteria
    {
        public WorkflowStatus? Status { get; set; }
        public string ModuleType { get; set; }
        public DateTime? StartTimeFrom { get; set; }
        public DateTime? StartTimeTo { get; set; }
        public Guid? WorkflowId { get; set; }
    }

    #endregion
}
