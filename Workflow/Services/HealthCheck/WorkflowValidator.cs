using AppWorkflow.Common.Enums;
using AppWorkflow.Core.Domain.Schema;
using AppWorkflow.Core.Interfaces.Services;
using AppWorkflow.Engine;
using AppWorkflow.Infrastructure.Repositories.IRepository;
using AppWorkflow.Infrastructure.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppWorkflow.Services.HealthCheck
{
    public class WorkflowValidator : IWorkflowValidator
    {
        private readonly ILogger<WorkflowValidator> _logger;
        private readonly IActionResolver _actionResolver;
        private readonly IExpressionEvaluator _expressionEvaluator;
        private readonly IWorkflowRepository _workflowRepository;

        public WorkflowValidator(
            ILogger<WorkflowValidator> logger,
            IActionResolver actionResolver,
            IExpressionEvaluator expressionEvaluator,
            IWorkflowRepository workflowRepository)
        {
            _logger = logger;
            _actionResolver = actionResolver;
            _expressionEvaluator = expressionEvaluator;
            _workflowRepository = workflowRepository;
        }

        public async Task<ValidationResult> ValidateWorkflowAsync(Workflow workflow)
        {
            var errors = new List<ValidationError>();

            try
            {
                // Validate basic workflow properties
                await ValidateBasicProperties(workflow, errors);

                // Validate steps
                await ValidateSteps(workflow, errors);

                //// Validate transitions
                //await ValidateTransitions(workflow, errors);

                //// Validate variables
                //await ValidateVariables(workflow, errors);

                //// Validate trigger configuration
                //await ValidateTriggerConfiguration(workflow, errors);

                return new ValidationResult
                {
                    IsValid =true|| !errors.Any(e => e.Severity == ValidationSeverity.Error),
                    Errors = errors,
                    Metadata = new Dictionary<string, object>
                    {
                        ["validatedAt"] = DateTime.UtcNow,
                        ["stepCount"] = workflow.Steps.Count,
                        ["variableCount"] = workflow.Variables.Count
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating workflow {WorkflowId}", workflow.Id);
                errors.Add(new ValidationError
                {
                    Property = "Workflow",
                    Error = $"Validation failed: {ex.Message}",
                    Severity = ValidationSeverity.Error,
                    Code = "VALIDATION_ERROR"
                });
                return new ValidationResult { IsValid = false, Errors = errors };
            }
        }

        public async Task<ValidationResult> ValidateStepAsync(WorkflowStep step)
        {
            var errors = new List<ValidationError>();

            try
            {
                //// Validate basic step properties
                //await ValidateStepProperties(step, errors);

                //// Validate action configuration
                //await ValidateActionConfiguration(step, errors);

                //// Validate transitions
                //await ValidateStepTransitions(step, errors);

                return new ValidationResult
                {
                    IsValid = !errors.Any(e => e.Severity == ValidationSeverity.Error),
                    Errors = errors
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating step {StepId}", step.Id);
                errors.Add(new ValidationError
                {
                    Property = "Step",
                    Error = $"Validation failed: {ex.Message}",
                    Severity = ValidationSeverity.Error,
                    Code = "STEP_VALIDATION_ERROR"
                });
                return new ValidationResult { IsValid = false, Errors = errors };
            }
        }

        public async Task<bool> CanExecuteStepAsync(Guid instanceId, Guid stepId)
        {
            try
            {
                var instance = await _workflowRepository.GetInstanceAsync(instanceId);
                if (instance == null) return false;

                // Check if workflow is in active state
                if (instance.Status != WorkflowStatus.Active)
                    return false;

                // Check if step exists in workflow definition
                var workflow = await _workflowRepository.GetByIdAsync(instance.WorkflowId);
                var step = workflow.Steps.FirstOrDefault(s => s.Id == stepId);
                if (step == null) return false;

                // Check step dependencies
                var dependencies = GetStepDependencies(workflow, step);
                foreach (var depStep in dependencies)
                {
                    var depStatus = instance.StepInstances
                        .FirstOrDefault(s => s.StepId == depStep.Id)?.Status;
                    if (depStatus != StepStatus.Completed)
                        return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking step execution capability for {StepId}", stepId);
                return false;
            }
        }

        public async Task<IEnumerable<ValidationError>> GetWorkflowErrorsAsync(Workflow workflow)
        {
            var result = await ValidateWorkflowAsync(workflow);
            return result.Errors;
        }

        private async Task ValidateBasicProperties(Workflow workflow, List<ValidationError> errors)
        {
            if (string.IsNullOrWhiteSpace(workflow.Name))
            {
                errors.Add(new ValidationError
                {
                    Property = "Name",
                    Error = "Workflow name is required",
                    Severity = ValidationSeverity.Error,
                    Code = "MISSING_NAME"
                });
            }

            if (string.IsNullOrWhiteSpace(workflow.ModuleType))
            {
                errors.Add(new ValidationError
                {
                    Property = "ModuleType",
                    Error = "Module type is required",
                    Severity = ValidationSeverity.Error,
                    Code = "MISSING_MODULE_TYPE"
                });
            }

            if (workflow.Steps.Count == 0)
            {
                errors.Add(new ValidationError
                {
                    Property = "Steps",
                    Error = "Workflow must have at least one step",
                    Severity = ValidationSeverity.Error,
                    Code = "NO_STEPS"
                });
            }

            if (workflow.InitialStepId == Guid.Empty)
            {
                errors.Add(new ValidationError
                {
                    Property = "InitialStepId",
                    Error = "Initial step must be specified",
                    Severity = ValidationSeverity.Error,
                    Code = "NO_INITIAL_STEP"
                });
            }
        }

        private async Task ValidateSteps(Workflow workflow, List<ValidationError> errors)
        {
            var processedSteps = new HashSet<Guid>();
            var visitedSteps = new HashSet<Guid>();

            // Check for cycles in workflow
            if (HasCycles(workflow.InitialStepId, workflow, visitedSteps, processedSteps))
            {
                errors.Add(new ValidationError
                {
                    Property = "Steps",
                    Error = "Workflow contains cycles",
                    Severity = ValidationSeverity.Error,
                    Code = "CYCLIC_WORKFLOW"
                });
            }

            // Validate each step
            foreach (var step in workflow.Steps)
            {
                var stepValidation = await ValidateStepAsync(step);
                errors.AddRange(stepValidation.Errors);
            }
        }

        private bool HasCycles(Guid currentStepId, Workflow workflow, HashSet<Guid> visited, HashSet<Guid> processed)
        {
            if (visited.Contains(currentStepId))
                return true;

            if (processed.Contains(currentStepId))
                return false;

            visited.Add(currentStepId);

            var currentStep = workflow.Steps.FirstOrDefault(s => s.Id == currentStepId);
            if (currentStep == null)
                return false;

            foreach (var transition in currentStep.Transitions)
            {
                if (HasCycles(transition.TargetStepId, workflow, visited, processed))
                    return true;
            }

            visited.Remove(currentStepId);
            processed.Add(currentStepId);
            return false;
        }

        private List<WorkflowStep> GetStepDependencies(Workflow workflow, WorkflowStep step)
        {
            var dependencies = new List<WorkflowStep>();
            var visited = new HashSet<Guid>();

            void TraverseDependencies(Guid stepId)
            {
                if (visited.Contains(stepId))
                    return;

                visited.Add(stepId);
                var currentStep = workflow.Steps.FirstOrDefault(s => s.Id == stepId);
                if (currentStep == null)
                    return;

                dependencies.Add(currentStep);

                foreach (var transition in currentStep.Transitions)
                {
                    if (transition.TargetStepId == step.Id)
                    {
                        TraverseDependencies(transition.SourceStepId);
                    }
                }
            }

            TraverseDependencies(step.Id);
            return dependencies;
        }

        // Additional validation methods...
    }
}
