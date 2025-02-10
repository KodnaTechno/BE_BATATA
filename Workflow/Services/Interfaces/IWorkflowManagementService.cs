using AppWorkflow.Common.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppWorkflow.Services.Interfaces
{
    /// <summary>
    /// Interface for managing workflow lifecycle and operations
    /// </summary>
    public interface IWorkflowManagementService
    {
        /// <summary>
        /// Creates a new workflow based on the provided definition
        /// </summary>
        /// <param name="createDto">The workflow creation data</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The created workflow details</returns>
        Task<WorkflowDto> CreateWorkflowAsync(CreateWorkflowDto createDto, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates an existing workflow
        /// </summary>
        /// <param name="workflowId">ID of the workflow to update</param>
        /// <param name="updateDto">The workflow update data</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The updated workflow details</returns>
        Task<WorkflowDto> UpdateWorkflowAsync(Guid workflowId, UpdateWorkflowDto updateDto, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a workflow by its ID
        /// </summary>
        /// <param name="workflowId">ID of the workflow to retrieve</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The workflow details</returns>
        Task<WorkflowDto> GetWorkflowAsync(Guid workflowId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a list of workflows with optional filtering
        /// </summary>
        /// <param name="page">Page number for pagination</param>
        /// <param name="pageSize">Number of items per page</param>
        /// <param name="searchTerm">Optional search term to filter workflows</param>
        /// <param name="status">Optional status filter</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Collection of workflow list items</returns>
        Task<IEnumerable<WorkflowListItemDto>> GetWorkflowsAsync(
            int page = 1,
            int pageSize = 20,
            string searchTerm = null,
            WorkflowStatus? status = null,
            CancellationToken cancellationToken = default);

        // Additional methods to consider adding:

        ///// <summary>
        ///// Deletes a workflow by its ID
        ///// </summary>
        //Task DeleteWorkflowAsync(Guid workflowId, CancellationToken cancellationToken = default);

        ///// <summary>
        ///// Publishes a workflow, making it available for execution
        ///// </summary>
        //Task<WorkflowDto> PublishWorkflowAsync(Guid workflowId, CancellationToken cancellationToken = default);

        ///// <summary>
        ///// Deactivates a workflow, preventing new instances from starting
        ///// </summary>
        //Task<WorkflowDto> DeactivateWorkflowAsync(Guid workflowId, CancellationToken cancellationToken = default);

        ///// <summary>
        ///// Retrieves a specific version of a workflow
        ///// </summary>
        //Task<WorkflowDto> GetWorkflowVersionAsync(Guid workflowId, int version, CancellationToken cancellationToken = default);

        ///// <summary>
        ///// Gets the version history of a workflow
        ///// </summary>

        ///// <summary>
        ///// Clones an existing workflow
        ///// </summary>
        //Task<WorkflowDto> CloneWorkflowAsync(Guid sourceWorkflowId, string newName, CancellationToken cancellationToken = default);

        ///// <summary>
        ///// Validates a workflow definition without creating it
        ///// </summary>
    }
}
