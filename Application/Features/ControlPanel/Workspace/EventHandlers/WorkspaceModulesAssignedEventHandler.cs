using Events.Modules.Workspace;
using Hangfire;
using MediatR;

namespace Application.Features.ControlPanel.Workspace.EventHandlers
{
    public class WorkspaceModulesAssignedEventHandler
        : INotificationHandler<WorkspaceModulesAssignedEvent>
    {
        private readonly IBackgroundJobClient _backgroundJobClient;

        public WorkspaceModulesAssignedEventHandler(IBackgroundJobClient backgroundJobClient)
        {
            _backgroundJobClient = backgroundJobClient;
        }

        public Task Handle(WorkspaceModulesAssignedEvent notification,
                           CancellationToken cancellationToken)
        {
            // Enqueue background job that will add/remove actions 
            _backgroundJobClient.Enqueue<IWorkspaceJob>(job =>
                job.ProcessWorkspaceModulesAssignedEvent(notification));

            return Task.CompletedTask;
        }
    }
}
