using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppWorkflow.Common.Exceptions
{
    public class ExpressionEvaluationException : WorkflowException
    {
        public ExpressionEvaluationException(string message, Exception innerException) : base(message, innerException)
        {
        }
       
    }
}
