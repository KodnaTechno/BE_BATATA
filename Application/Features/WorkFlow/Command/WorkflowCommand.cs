using AppCommon.DTOs.Modules;
using Application.Common.Models;
using Application.Interfaces;
using Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.WorkFlow.Command
{
    public class WorkflowCommand : BaseCommand<WorkflowCommandResult>
    {
        public Guid ActionId { get; set; }
        public string ModuleType { get; set; }
        public List<PropertyDataDto> PropertiesData{ get; set; }
        // Any other metadata needed

        public override IEvent GetEvent(ApiResponse<WorkflowCommandResult> response)
        {
            // Generate appropriate event based on action type and result
            return null; // Or create appropriate event
        }
    }

}
