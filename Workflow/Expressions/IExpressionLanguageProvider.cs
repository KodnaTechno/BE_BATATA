namespace AppWorkflow.Expressions;

using System.Text;

public interface IExpressionLanguageProvider
    {
        Task<T> EvaluateAsync<T>(string expression, IDictionary<string, object> context);
        Task<bool> ValidateExpressionAsync(string expression);
        Task<string> GetExpressionSyntaxAsync();
    }