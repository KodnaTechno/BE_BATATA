using Events.Modules.Properties;
using Hangfire;
using MediatR;

namespace Application.Features.ControlPanel.Properties.EventHandlers
{
    public class PropertyDeletedEventHandler : INotificationHandler<PropertyDeletedEvent>
    {
        private readonly IBackgroundJobClient _backgroundJobClient;

        public PropertyDeletedEventHandler(IBackgroundJobClient backgroundJobClient)
        {
            _backgroundJobClient = backgroundJobClient;
        }

        public Task Handle(PropertyDeletedEvent notification, CancellationToken cancellationToken)
        {
            _backgroundJobClient.Enqueue<IPropertyJob>(job => job.ProcessPropertyDeletedEvent(notification));

            return Task.CompletedTask;
        }
    }
}
