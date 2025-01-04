namespace AppWorkflow.Infrastructure.Triggers;

using AppWorkflow.Core.Interfaces.Services;
using AppWorkflow.Infrastructure.Data.Configurations;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;

public class ScheduledTrigger : IWorkflowTrigger
    {
        private readonly ILogger<ScheduledTrigger> _logger;
        private readonly IWorkflowEngine _workflowEngine;
       // private readonly ISchedulerService _scheduler;

        public string TriggerType => "Scheduled";

        public async Task<bool> EvaluateAsync(TriggerContext context)
        {
            // Scheduled triggers always evaluate to true when they're triggered
            return true;
        }

        public async Task SubscribeAsync(TriggerConfiguration config)
        {
            //var scheduleConfig = config.Configuration.Deserialize<ScheduledTriggerConfig>();

            //await _scheduler.ScheduleWorkflowAsync(
            //    config.WorkflowId,
            //    scheduleConfig.CronExpression,
            //    scheduleConfig.TimeZone
            //);
        }

        public async Task UnsubscribeAsync(Guid workflowId)
        {
            //await _scheduler.UnscheduleWorkflowAsync(workflowId);
        }
    }