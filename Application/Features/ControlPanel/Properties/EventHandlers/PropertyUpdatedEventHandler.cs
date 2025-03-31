using Events.Modules.Properties;
using Hangfire;
using MediatR;

namespace Application.Features.ControlPanel.Properties.EventHandlers
{
    public class PropertyUpdatedEventHandler : INotificationHandler<PropertyUpdatedEvent>
    {
        private readonly IBackgroundJobClient _backgroundJobClient;

        public PropertyUpdatedEventHandler(IBackgroundJobClient backgroundJobClient)
        {
            _backgroundJobClient = backgroundJobClient;
        }

        public Task Handle(PropertyUpdatedEvent notification, CancellationToken cancellationToken)
        {
            _backgroundJobClient.Enqueue<IPropertyJob>(job => job.ProcessPropertyUpdatedEvent(notification));

            return Task.CompletedTask;
        }
    }
}
