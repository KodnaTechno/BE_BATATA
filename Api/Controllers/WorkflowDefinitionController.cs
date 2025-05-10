using AppWorkflow.Common.DTO;
using AppWorkflow.Common.Enums;
using AppWorkflow.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Controllers;

[ApiController]
[Route("api/workflow/definitions")]
public class WorkflowDefinitionController : ControllerBase
{
    private readonly IWorkflowManagementService _workflowManagementService;

    public WorkflowDefinitionController(IWorkflowManagementService workflowManagementService)
    {
        _workflowManagementService = workflowManagementService;
    }

    // Create a new workflow definition (global or module)
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateWorkflowDto dto, CancellationToken cancellationToken)
    {
        var result = await _workflowManagementService.CreateWorkflowAsync(dto, cancellationToken);
        return Ok(result);
    }

    // Update an existing workflow definition
    [HttpPut("{workflowId}")]
    public async Task<IActionResult> Update(Guid workflowId, [FromBody] UpdateWorkflowDto dto, CancellationToken cancellationToken)
    {
        var result = await _workflowManagementService.UpdateWorkflowAsync(workflowId, dto, cancellationToken);
        return Ok(result);
    }

    // Get a workflow definition by ID
    [HttpGet("{workflowId}")]
    public async Task<IActionResult> Get(Guid workflowId, CancellationToken cancellationToken)
    {
        var result = await _workflowManagementService.GetWorkflowAsync(workflowId, cancellationToken);
        return Ok(result);
    }

    // List all workflow definitions (optionally filter by module/global)
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string searchTerm = null, [FromQuery] WorkflowStatus? status = null, CancellationToken cancellationToken = default)
    {
        var result = await _workflowManagementService.GetWorkflowsAsync(page, pageSize, searchTerm, status, cancellationToken);
        return Ok(result);
    }

    // Delete a workflow definition
    [HttpDelete("{workflowId}")]
    public async Task<IActionResult> Delete(Guid workflowId, CancellationToken cancellationToken)
    {
        await _workflowManagementService.DeleteWorkflowAsync(workflowId, cancellationToken);
        return NoContent();
    }

    // Publish a workflow definition
    [HttpPost("{workflowId}/publish")]
    public async Task<IActionResult> Publish(Guid workflowId, CancellationToken cancellationToken)
    {
        var result = await _workflowManagementService.PublishWorkflowAsync(workflowId, cancellationToken);
        return Ok(result);
    }

    // Deactivate a workflow definition
    [HttpPost("{workflowId}/deactivate")]
    public async Task<IActionResult> Deactivate(Guid workflowId, CancellationToken cancellationToken)
    {
        var result = await _workflowManagementService.DeactivateWorkflowAsync(workflowId, cancellationToken);
        return Ok(result);
    }

    // Get version history for a workflow definition
    [HttpGet("{workflowId}/versions")]
    public async Task<IActionResult> GetVersionHistory(Guid workflowId, CancellationToken cancellationToken)
    {
        var result = await _workflowManagementService.GetWorkflowVersionHistoryAsync(workflowId, cancellationToken);
        return Ok(result);
    }

    // Clone/Create a new version of a workflow definition
    [HttpPost("{workflowId}/clone")]
    public async Task<IActionResult> Clone(Guid workflowId, CancellationToken cancellationToken)
    {
        var result = await _workflowManagementService.CloneWorkflowAsync(workflowId, cancellationToken);
        return Ok(result);
    }
}
