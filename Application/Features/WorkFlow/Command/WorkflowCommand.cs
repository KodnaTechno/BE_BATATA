using AppCommon.DTOs;
using AppCommon.DTOs.Modules;
using Application.Interfaces;
using Events;
using Application.Features.WorkFlow.Command;
using WorkflowCommandResultApp = Application.Features.WorkFlow.Command.WorkflowCommandResult;

namespace Application.Features.WorkFlow.Command
{
    public class WorkflowCommand : BaseCommand<WorkflowCommandResultApp>
    {
        public Guid ActionId { get; set; }
        public string ModuleType { get; set; }
        public List<PropertyDataDto> PropertiesData{ get; set; }
        // Any other metadata needed

        public override IEvent GetEvent(ApiResponse<WorkflowCommandResultApp> response)
        {
            // Generate appropriate event based on action type and result
            return null; // Or create appropriate event
        }
    }

}
