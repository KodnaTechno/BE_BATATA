namespace AppWorkflow.Engine;

using AppWorkflow.Core.Domain.Schema;
using AppWorkflow.Infrastructure.Data.Context;
using System.Text;

public interface IStepExecutor
    {
        Task<StepExecutionResult> ExecuteAsync(WorkflowExecutionContext context, WorkflowStep step);
        Task<bool> ValidateAsync(WorkflowStep step);
        Task<object> GetConfigurationSchemaAsync();
    }
    public interface IStepExecutorFactory
    {
        IStepExecutor CreateExecutor(string stepType);
        void RegisterExecutor<T>(string stepType) where T : IStepExecutor;
        bool HasExecutor(string stepType);
    }