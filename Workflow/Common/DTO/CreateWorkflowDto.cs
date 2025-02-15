using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppWorkflow.Common.DTO
{
    public class CreateWorkflowDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<CreateWorkflowStepDto> Steps { get; set; }
        public Dictionary<string, string> Metadata { get; set; }
        public List<CreateWorkflowTriggerDto> Triggers { get; set; }
    }

    public class CreateWorkflowStepDto
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public JsonDocument Configuration { get; set; }
        public List<CreateStepTransitionDto> Transitions { get; set; }
    }

    public class CreateStepTransitionDto
    {
        public string TargetStepName { get; set; }
        public string Condition { get; set; }
    }

    public class CreateWorkflowVariableDto
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public object DefaultValue { get; set; }
    }

    public class CreateWorkflowTriggerDto
    {
        public string Type { get; set; }
        public Dictionary<string, string> Configuration { get; set; }
    }

    public class UpdateWorkflowDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<CreateWorkflowStepDto> Steps { get; set; }
        public List<CreateWorkflowVariableDto> Variables { get; set; }
        public Dictionary<string, string> Metadata { get; set; }
        public List<CreateWorkflowTriggerDto> Triggers { get; set; }
    }

    public class WorkflowDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public WorkflowStatus Status { get; set; }
        public string Version { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? PublishedAt { get; set; }
        public List<WorkflowStepDto> Steps { get; set; }
        public List<WorkflowVariableDto> Variables { get; set; }
        public Dictionary<string, string> Metadata { get; set; }
        public List<WorkflowTriggerDto> Triggers { get; set; }
    }

    public class WorkflowStepDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public Dictionary<string, object> Configuration { get; set; }
        public List<StepTransitionDto> Transitions { get; set; }
    }

    public class StepTransitionDto
    {
        public Guid TargetStepId { get; set; }
        public string Condition { get; set; }
    }

    public class WorkflowVariableDto
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public object DefaultValue { get; set; }
    }

    public class WorkflowTriggerDto
    {
        public string Type { get; set; }
        public Dictionary<string, string> Configuration { get; set; }
    }

    public class WorkflowListItemDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public WorkflowStatus Status { get; set; }
        public string Version { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
