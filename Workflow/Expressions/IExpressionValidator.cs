namespace AppWorkflow.Expressions;

public interface IExpressionValidator
{
    Task<bool> ValidateAsync(string expression);
}