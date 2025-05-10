using AppCommon.DTOs;
using AppWorkflow.Core.Interfaces.Services;
using AppWorkflow.Services.Monitoring;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [Route("api/workflow/monitoring")]
    [ApiController]
    public class WorkflowMonitoringController : ControllerBase
    {
        private readonly IWorkflowMonitoringService _monitoringService;
        private readonly ILogger<WorkflowMonitoringController> _logger;

        public WorkflowMonitoringController(
            IWorkflowMonitoringService monitoringService,
            ILogger<WorkflowMonitoringController> logger)
        {
            _monitoringService = monitoringService;
            _logger = logger;
        }

        [HttpGet("dashboard/summary")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<WorkflowMonitoringSummary>>> GetDashboardSummary()
        {
            try
            {
                var summary = await _monitoringService.GetDashboardSummaryAsync();
                return ApiResponse<WorkflowMonitoringSummary>.Success(summary);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving workflow monitoring dashboard summary");
                return ApiResponse<WorkflowMonitoringSummary>.Fail(
                    "MONITORING_ERROR",
                    "An error occurred while retrieving the monitoring summary");
            }
        }

        [HttpGet("instances/active")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<IEnumerable<WorkflowInstanceSummary>>>> GetActiveInstances(
            [FromQuery] int maxResults = 100)
        {
            try
            {
                var instances = await _monitoringService.GetActiveWorkflowInstancesAsync(maxResults);
                return ApiResponse<IEnumerable<WorkflowInstanceSummary>>.Success(instances);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving active workflow instances");
                return ApiResponse<IEnumerable<WorkflowInstanceSummary>>.Fail(
                    "MONITORING_ERROR",
                    "An error occurred while retrieving active instances");
            }
        }

        [HttpGet("instances/recent")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<IEnumerable<WorkflowInstanceSummary>>>> GetRecentInstances(
            [FromQuery] int maxResults = 100)
        {
            try
            {
                var instances = await _monitoringService.GetRecentWorkflowInstancesAsync(maxResults);
                return ApiResponse<IEnumerable<WorkflowInstanceSummary>>.Success(instances);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving recent workflow instances");
                return ApiResponse<IEnumerable<WorkflowInstanceSummary>>.Fail(
                    "MONITORING_ERROR",
                    "An error occurred while retrieving recent instances");
            }
        }

        [HttpGet("instances/failed")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<IEnumerable<WorkflowInstanceSummary>>>> GetFailedInstances(
            [FromQuery] int maxResults = 100)
        {
            try
            {
                var instances = await _monitoringService.GetFailedWorkflowInstancesAsync(maxResults);
                return ApiResponse<IEnumerable<WorkflowInstanceSummary>>.Success(instances);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving failed workflow instances");
                return ApiResponse<IEnumerable<WorkflowInstanceSummary>>.Fail(
                    "MONITORING_ERROR",
                    "An error occurred while retrieving failed instances");
            }
        }

        [HttpGet("instances/{instanceId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<WorkflowInstanceDetails>>> GetInstanceDetails(Guid instanceId)
        {
            try
            {
                var details = await _monitoringService.GetWorkflowInstanceDetailsAsync(instanceId);
                if (details == null)
                {
                    return ApiResponse<WorkflowInstanceDetails>.Fail(
                        "NOT_FOUND",
                        $"Workflow instance with ID {instanceId} not found");
                }
                return ApiResponse<WorkflowInstanceDetails>.Success(details);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving workflow instance details for {InstanceId}", instanceId);
                return ApiResponse<WorkflowInstanceDetails>.Fail(
                    "MONITORING_ERROR",
                    "An error occurred while retrieving instance details");
            }
        }

        [HttpGet("performance")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<WorkflowPerformanceMetrics>>> GetPerformanceMetrics(
            [FromQuery] Guid? workflowId = null)
        {
            try
            {
                var metrics = await _monitoringService.GetPerformanceMetricsAsync(workflowId);
                return ApiResponse<WorkflowPerformanceMetrics>.Success(metrics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving workflow performance metrics");
                return ApiResponse<WorkflowPerformanceMetrics>.Fail(
                    "MONITORING_ERROR",
                    "An error occurred while retrieving performance metrics");
            }
        }

        [HttpPost("search")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<IEnumerable<WorkflowInstanceSummary>>>> SearchInstances(
            [FromBody] WorkflowSearchCriteria criteria)
        {
            try
            {
                var instances = await _monitoringService.SearchWorkflowInstancesAsync(criteria);
                return ApiResponse<IEnumerable<WorkflowInstanceSummary>>.Success(instances);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching workflow instances");
                return ApiResponse<IEnumerable<WorkflowInstanceSummary>>.Fail(
                    "MONITORING_ERROR",
                    "An error occurred while searching instances");
            }
        }
    }
}
