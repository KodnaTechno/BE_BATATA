using AppWorkflow.Infrastructure.Services.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppWorkflow.Infrastructure.Services
{
    public interface IApprovalTargetResolver
    {
        Task<List<string>> ResolveTargetUsersAsync(ActionContext context);
    }
}
