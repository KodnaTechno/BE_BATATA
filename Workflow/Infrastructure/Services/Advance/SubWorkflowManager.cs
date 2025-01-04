using AppWorkflow.Core.Domain.Data;
using AppWorkflow.Core.Interfaces.Services;
using AppWorkflow.Infrastructure.Repositories.IRepository;


namespace AppWorkflow.Infrastructure.Services.Advance
{
    public class SubWorkflowManager
    {
        private readonly IWorkflowEngine _workflowEngine;
        private readonly IWorkflowRepository _workflowRepository;
        private readonly IWorkflowDataRepository _instanceRepository;
        private readonly ILogger<SubWorkflowManager> _logger;

        public SubWorkflowManager(
            IWorkflowEngine workflowEngine,
            IWorkflowRepository workflowRepository,
            IWorkflowDataRepository instanceRepository,
            ILogger<SubWorkflowManager> logger)
        {
            _workflowEngine = workflowEngine;
            _workflowRepository = workflowRepository;
            _instanceRepository = instanceRepository;
            _logger = logger;
        }

        public async Task<WorkflowData> StartSubWorkflowAsync(
            Guid parentInstanceId,
            Guid subWorkflowId,
            dynamic moduleData,
            Dictionary<string, object> inheritedVariables = null)
        {
            try
            {
                var parentInstance = await _instanceRepository.GetByIdAsync(parentInstanceId);
                if (parentInstance == null)
                    throw new WorkflowNotFoundException(message: $"Parent workflow instance {parentInstanceId} not found");

                // Create sub-workflow instance with parent reference
                var subWorkflowInstance = await _workflowEngine.StartWorkflowAsync(
                    subWorkflowId,
                    moduleData,
                    inheritedVariables);

                // Create parent-child relationship
                var relation = new WorkflowRelation
                {
                    ParentInstanceId = parentInstanceId,
                    ChildInstanceId = subWorkflowInstance.Id,
                    RelationType = WorkflowRelationType.SubWorkflow,
                    CreatedAt = DateTime.UtcNow
                };

                await _instanceRepository.AddWorkflowRelationAsync(relation);

                _logger.LogInformation(
                    "Started sub-workflow {SubWorkflowId} for parent {ParentInstanceId}",
                    subWorkflowId,
                    parentInstanceId);

                return subWorkflowInstance;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error starting sub-workflow {SubWorkflowId} for parent {ParentInstanceId}",
                    subWorkflowId,
                    parentInstanceId);
                throw;
            }
        }

        public async Task<bool> IsSubWorkflowCompletedAsync(Guid parentInstanceId, Guid childInstanceId)
        {
            var childInstance = await _instanceRepository.GetByIdAsync(childInstanceId);
            return childInstance?.Status == WorkflowStatus.Completed;
        }

        public async Task<IEnumerable<WorkflowData>> GetSubWorkflowsAsync(Guid parentInstanceId)
        {
            var relations = await _instanceRepository.GetWorkflowRelationsAsync(parentInstanceId);
            var subWorkflowIds = relations
                .Where(r => r.RelationType == WorkflowRelationType.SubWorkflow)
                .Select(r => r.ChildInstanceId);

            var subWorkflows = new List<WorkflowData>();
            foreach (var id in subWorkflowIds)
            {
                var instance = await _instanceRepository.GetByIdAsync(id);
                if (instance != null)
                {
                    subWorkflows.Add(instance);
                }
            }

            return subWorkflows;
        }
    }
}
