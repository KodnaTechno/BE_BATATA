namespace AppWorkflow.Infrastructure.Data.Context;

using System.Text;

public class EvaluationContext
    {
        public IDictionary<string, object> Variables { get; set; }
        public IDictionary<string, Func<object[], Task<object>>> Functions { get; set; }
    }