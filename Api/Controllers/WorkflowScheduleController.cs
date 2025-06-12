using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using AppWorkflow.Infrastructure.Data.Configurations;
using AppWorkflow.Infrastructure.Repositories.IRepository;
using AppWorkflow.Infrastructure.Triggers;
using Microsoft.AspNetCore.Http;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WorkflowScheduleController : ControllerBase
    {
        private readonly IWorkflowRepository _workflowRepository;
        private readonly TriggerManager _triggerManager;

        public WorkflowScheduleController(IWorkflowRepository workflowRepository, TriggerManager triggerManager)
        {
            _workflowRepository = workflowRepository;
            _triggerManager = triggerManager;
        }

        [HttpPost("workflows/{workflowId}/schedules")]
        public async Task<IActionResult> AddSchedule(Guid workflowId, [FromBody] TriggerConfiguration schedule)
        {
            var workflow = await _workflowRepository.GetByIdAsync(workflowId);
            if (workflow == null) return NotFound();
            if (workflow.TriggerConfigs == null) workflow.TriggerConfigs = new List<TriggerConfiguration>();
            schedule.Id = Guid.NewGuid();
            schedule.WorkflowId = workflowId;
            workflow.TriggerConfigs.Add(schedule);
            await _workflowRepository.UpdateAsync(workflow);
            await _triggerManager.RegisterTriggerAsync(schedule);
            return Ok(schedule);
        }

        [HttpGet("workflows/{workflowId}/schedules")]
        public async Task<IActionResult> GetSchedules(Guid workflowId)
        {
            var workflow = await _workflowRepository.GetByIdAsync(workflowId);
            if (workflow == null) return NotFound();
            var schedules = workflow.TriggerConfigs?.Where(tc => !string.IsNullOrEmpty(tc.Schedule)).ToList() ?? new List<TriggerConfiguration>();
            return Ok(schedules);
        }

        [HttpDelete("workflows/{workflowId}/schedules/{scheduleId}")]
        public async Task<IActionResult> DeleteSchedule(Guid workflowId, Guid scheduleId)
        {
            var workflow = await _workflowRepository.GetByIdAsync(workflowId);
            if (workflow == null) return NotFound();
            var schedule = workflow.TriggerConfigs?.FirstOrDefault(tc => tc.Id == scheduleId);
            if (schedule == null) return NotFound();
            workflow.TriggerConfigs.Remove(schedule);
            await _workflowRepository.UpdateAsync(workflow);
            await _triggerManager.UnregisterTriggersAsync(workflowId);
            return NoContent();
        }

        [HttpPost("workflows/{workflowId}/triggers")]
        public async Task<IActionResult> AddTrigger(Guid workflowId, [FromBody] TriggerConfiguration trigger)
        {
            var workflow = await _workflowRepository.GetByIdAsync(workflowId);
            if (workflow == null) return NotFound();
            if (workflow.TriggerConfigs == null) workflow.TriggerConfigs = new List<TriggerConfiguration>();
            trigger.Id = Guid.NewGuid();
            trigger.WorkflowId = workflowId;
            workflow.TriggerConfigs.Add(trigger);
            await _workflowRepository.UpdateAsync(workflow);
            await _triggerManager.RegisterTriggerAsync(trigger);
            return Ok(trigger);
        }

        [HttpGet("workflows/{workflowId}/triggers")]
        public async Task<IActionResult> GetTriggers(Guid workflowId)
        {
            var workflow = await _workflowRepository.GetByIdAsync(workflowId);
            if (workflow == null) return NotFound();
            var triggers = workflow.TriggerConfigs?.Where(tc => string.IsNullOrEmpty(tc.Schedule)).ToList() ?? new List<TriggerConfiguration>();
            return Ok(triggers);
        }

        [HttpDelete("workflows/{workflowId}/triggers/{triggerId}")]
        public async Task<IActionResult> DeleteTrigger(Guid workflowId, Guid triggerId)
        {
            var workflow = await _workflowRepository.GetByIdAsync(workflowId);
            if (workflow == null) return NotFound();
            var trigger = workflow.TriggerConfigs?.FirstOrDefault(tc => tc.Id == triggerId);
            if (trigger == null) return NotFound();
            workflow.TriggerConfigs.Remove(trigger);
            await _workflowRepository.UpdateAsync(workflow);
            await _triggerManager.UnregisterTriggersAsync(workflowId);
            return NoContent();
        }
    }
}
