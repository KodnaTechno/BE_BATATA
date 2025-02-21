using Module.Domain.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Helpers
{
    public static class GetWorkFlowActionTypeForAppAction
    {
        public static string GetWorkFlowActionType(this ActionType appAction)
        {
            return appAction switch
            {
                ActionType.Create => "CreateModuleAction",
                ActionType.Update => "UpdateModuleAction",
                ActionType.Delete => "DeleteModuleAction",
                _ => throw new NotImplementedException()
            };
        }
    }
}
