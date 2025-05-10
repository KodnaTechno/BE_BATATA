namespace AppWorkflow.Engine;

using AppWorkflow.Infrastructure.Data.Context;
using System.Text;
using AppWorkflow.Domain.Schema;

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
    public class StepExecutorFactory: IStepExecutorFactory
{
    private readonly Dictionary<string, Type> _executors = new();
    private readonly IServiceProvider _serviceProvider;
    public StepExecutorFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    public IStepExecutor CreateExecutor(string stepType)
    {
        if (!_executors.TryGetValue(stepType, out var executorType))
        {
            throw new InvalidOperationException($"No executor registered for step type {stepType}");
        }
        return (IStepExecutor)_serviceProvider.GetRequiredService(executorType);
    }
    public void RegisterExecutor<T>(string stepType) where T : IStepExecutor
    {
        _executors[stepType] = typeof(T);
    }
    public bool HasExecutor(string stepType)
    {
        return _executors.ContainsKey(stepType);
    }
}
  