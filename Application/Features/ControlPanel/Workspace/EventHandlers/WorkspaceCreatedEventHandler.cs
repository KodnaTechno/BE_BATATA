using Events.Modules.Workspace;
using Hangfire;
using MediatR;

namespace Application.Features.ControlPanel.Workspace.EventHandlers
{
    public class WorkspaceCreatedEventHandler : INotificationHandler<WorkspaceCreatedEvent>
    {
        private readonly IBackgroundJobClient _backgroundJobClient;

        public WorkspaceCreatedEventHandler(IBackgroundJobClient backgroundJobClient)
        {
            _backgroundJobClient = backgroundJobClient;
        }

        public async Task Handle(WorkspaceCreatedEvent notification, CancellationToken cancellationToken)
        {
            _backgroundJobClient.Enqueue(() => Console.WriteLine($"Title is {notification.Title}"));

            await Task.CompletedTask;
        }
    }
}
