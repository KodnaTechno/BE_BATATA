namespace AppWorkflow.Engine;

using AppWorkflow.Common.Exceptions;
using AppWorkflow.Expressions;
using Microsoft.Extensions.Logging;

public class ExpressionEvaluator : IExpressionEvaluator
    {
        private readonly ILogger<ExpressionEvaluator> _logger;
        private readonly IExpressionLanguageProvider _expressionProvider;

        public async Task<T> EvaluateAsync<T>(string expression, object moduleData, IDictionary<string, object> variables)
        {
            try
            {
                var context = new Dictionary<string, object>
                {
                    ["module"] = moduleData,
                    ["variables"] = variables
                };

                return await _expressionProvider.EvaluateAsync<T>(expression, context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error evaluating expression: {expression}");
                throw new ExpressionEvaluationException(expression, ex);
            }
        }

        public async Task<object> EvaluateAsync(string expression, object moduleData, IDictionary<string, object> variables)
        {
            return await EvaluateAsync<object>(expression, moduleData, variables);
        }
    }