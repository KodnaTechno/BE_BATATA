using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Modules.Workspace
{
    public class WorkspaceModulesAssignedEvent : BaseEvent, ICreatedEvent
    {
        public Guid WorkspaceId { get; set; }
    }
}
