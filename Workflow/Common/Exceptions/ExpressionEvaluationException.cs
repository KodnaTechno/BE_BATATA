namespace AppWorkflow.Common.Exceptions;

using AppWorkflow.Infrastructure.Data.Context;
using System.Text;

public class ExpressionEvaluationException : Exception
    {
        public ExpressionEvaluationException(string message) : base(message) { }
        public ExpressionEvaluationException(string message, Exception innerException)
            : base(message, innerException) { }
    }

    // Example usage of custom expression parser (you might want to use a library like Flee or NCalc instead)
    public interface IExpressionParser
    {
        Task<IExpression> ParseAsync(string expression);
    }

    public interface IExpression
    {
        Task<object> EvaluateAsync(EvaluationContext context);
    }

    public interface IExpressionValidator
    {
        Task<bool> ValidateAsync(string expression);
    }