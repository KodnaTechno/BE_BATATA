//using Events.Modules.Workspace;
//using Hangfire;
//using MediatR;

//namespace Application.Features.ControlPanel.Workspace.EventHandlers
//{
//    public class WorkspaceModulesSetEventHandler : INotificationHandler<WorkspaceModulesSetEvent>
//    {
//        private readonly IBackgroundJobClient _backgroundJobClient;

//        public WorkspaceModulesSetEventHandler(IBackgroundJobClient backgroundJobClient)
//        {
//            _backgroundJobClient = backgroundJobClient;
//        }

//        public async Task Handle(WorkspaceModulesSetEvent notification, CancellationToken cancellationToken)
//        {
//            _backgroundJobClient.Enqueue(() => Console.WriteLine($"Title"));

//            await Task.CompletedTask;
//        }
//    }
//}
