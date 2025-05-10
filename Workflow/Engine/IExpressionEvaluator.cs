namespace AppWorkflow.Engine;

public interface IExpressionEvaluator
{
    Task<T> EvaluateAsync<T>(string expression, object moduleData, Dictionary<string, object> variables);
}