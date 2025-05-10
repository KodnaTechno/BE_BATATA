using System.Threading;
using System.Threading.Tasks;
using MediatR;
using AppWorkflow.Triggers;
using AppWorkflow.Infrastructure.Triggers;
using Microsoft.Extensions.Logging;
using Events;

namespace AppWorkflow.Infrastructure.Triggers
{
    public class EventTriggerListener : INotificationHandler<IEvent>
    {
        private readonly TriggerManager _triggerManager;
        private readonly ILogger<EventTriggerListener> _logger;

        public EventTriggerListener(TriggerManager triggerManager, ILogger<EventTriggerListener> logger)
        {
            _triggerManager = triggerManager;
            _logger = logger;
        }

        public async Task Handle(IEvent notification, CancellationToken cancellationToken)
        {
            var context = new TriggerContext
            {
                TriggerType = "Event",
                ModuleType = null,
                ModuleId = notification.EventId,
                Data = null,
                Metadata = new System.Collections.Generic.Dictionary<string, object>
                {
                    { "EventType", notification.GetType().Name },
                    { "CorrelationId", notification.CorrelationId },
                    { "UserId", notification.UserId }
                }
            };
            _logger.LogInformation($"EventTriggerListener: Handling event {notification.GetType().Name} with ID {notification.EventId}");
            await _triggerManager.HandleTriggerEventAsync(context);
        }
    }
}
