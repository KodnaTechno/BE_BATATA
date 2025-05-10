namespace AppWorkflow.Expressions;

using AppWorkflow.Infrastructure.Data.Context;
using System.Threading.Tasks;

public interface IParsedExpression
{
    Task<object> EvaluateAsync(EvaluationContext context);
}
