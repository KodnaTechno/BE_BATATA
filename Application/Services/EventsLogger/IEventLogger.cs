using Application.Interfaces;
using Events;
using System.Threading;

namespace Application.Services.EventsLogger
{
    public interface IEventLogger
    {
        Task LogEventAsync(IEvent @event, CancellationToken cancellationToken = default);
        Task UpdateEventStatusAsync(Guid eventId, string status, Exception exception = null, CancellationToken cancellationToken = default);
    }
}
