using Events.Modules.Properties;
using Hangfire;
using MediatR;

namespace Application.Features.ControlPanel.Properties.EventHandlers
{
    public class PropertyCreatedEventHandler : INotificationHandler<PropertyCreatedEvent>
    {
        private readonly IBackgroundJobClient _backgroundJobClient;

        public PropertyCreatedEventHandler(IBackgroundJobClient backgroundJobClient)
        {
            _backgroundJobClient = backgroundJobClient;
        }

        public Task Handle(PropertyCreatedEvent notification, CancellationToken cancellationToken)
        {
            _backgroundJobClient.Enqueue<IPropertyJob>(job => job.ProcessPropertyCreatedEvent(notification));

            return Task.CompletedTask;
        }
    }
}
