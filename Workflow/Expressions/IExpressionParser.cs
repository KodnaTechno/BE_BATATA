namespace AppWorkflow.Expressions;

public interface IExpressionParser
{
    object Parse(string expression);
    Task<IParsedExpression> ParseAsync(string expression);
}
