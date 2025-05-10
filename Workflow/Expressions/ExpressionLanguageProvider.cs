namespace AppWorkflow.Expressions;

using AppWorkflow.Infrastructure.Data.Context;
using Microsoft.Extensions.Logging;
using System.Text;

public class ExpressionLanguageProvider : IExpressionLanguageProvider
    {
        private readonly Microsoft.Extensions.Logging.ILogger<ExpressionLanguageProvider> _logger;
        private readonly IExpressionParser _parser;
        private readonly IExpressionValidator _validator;

        public ExpressionLanguageProvider(
            ILogger<ExpressionLanguageProvider> logger,
            IExpressionParser parser,
            IExpressionValidator validator)
        {
            _logger = logger;
            _parser = parser;
            _validator = validator;
        }

        public async Task<T> EvaluateAsync<T>(string expression, IDictionary<string, object> context)
        {
            try
            {
                // Parse the expression
                var parsedExpression = await _parser.ParseAsync(expression);

                // Create evaluation context
                var evaluationContext = new EvaluationContext
                {
                    Variables = context,
                    Functions = GetDefaultFunctions()
                };

                // Evaluate the expression
                var result = await parsedExpression.EvaluateAsync(evaluationContext);

                // Convert result to requested type
                return (T)Convert.ChangeType(result, typeof(T));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error evaluating expression: {Expression}", expression);
                throw new ExpressionEvaluationException($"Failed to evaluate expression: {expression}", ex);
            }
        }

        public async Task<bool> ValidateExpressionAsync(string expression)
        {
            try
            {
                return await _validator.ValidateAsync(expression);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating expression: {Expression}", expression);
                return false;
            }
        }

        public async Task<string> GetExpressionSyntaxAsync()
        {
            return @"
                Available Functions:
                - if(condition, trueValue, falseValue)
                - concat(str1, str2, ...)
                - sum(num1, num2, ...)
                - avg(num1, num2, ...)
                - count(array)
                - contains(array, value)
                - date(dateString)
                - now()
                - formatDate(date, format)
                
                Variable Access:
                - module.propertyName
                - variables.variableName
                
                Operators:
                - Arithmetic: +, -, *, /, %
                - Comparison: ==, !=, >, <, >=, <=
                - Logical: &&, ||, !
                - String: +
            ";
        }

        private Dictionary<string, Func<object[], Task<object>>> GetDefaultFunctions()
        {
            return new Dictionary<string, Func<object[], Task<object>>>
            {
                ["if"] = async (args) =>
                {
                    if (args.Length != 3)
                        throw new ArgumentException("If function requires 3 arguments");
                    return (bool)args[0] ? args[1] : args[2];
                },

                ["concat"] = async (args) =>
                {
                    return string.Concat(args.Select(a => a?.ToString() ?? ""));
                },

                ["sum"] = async (args) =>
                {
                    return args.Select(a => Convert.ToDouble(a)).Sum();
                },

                ["avg"] = async (args) =>
                {
                    var numbers = args.Select(a => Convert.ToDouble(a));
                    return numbers.Any() ? numbers.Average() : 0;
                },

                ["count"] = async (args) =>
                {
                    if (args[0] is IEnumerable<object> enumerable)
                        return enumerable.Count();
                    throw new ArgumentException("Count function requires an array");
                },

                ["contains"] = async (args) =>
                {
                    if (args.Length != 2 || !(args[0] is IEnumerable<object> enumerable))
                        throw new ArgumentException("Contains function requires an array and a value");
                    return enumerable.Contains(args[1]);
                },

                ["date"] = async (args) =>
                {
                    if (args.Length != 1)
                        throw new ArgumentException("Date function requires 1 argument");
                    return DateTime.Parse(args[0].ToString());
                },

                ["now"] = async (args) =>
                {
                    return DateTime.UtcNow;
                },

                ["formatDate"] = async (args) =>
                {
                    if (args.Length != 2)
                        throw new ArgumentException("FormatDate function requires 2 arguments");
                    var date = args[0] is DateTime dt ? dt : DateTime.Parse(args[0].ToString());
                    return date.ToString(args[1].ToString());
                }
            };
        }
    }