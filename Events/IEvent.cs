using MediatR;

namespace Events
{
    public interface IEvent : INotification
    {
        Guid EventId { get; }
        DateTime Timestamp { get; }
        string CorrelationId { get; set; }
        Guid UserId { get; set; }
    }

    public abstract class BaseEvent : IEvent
    {
        public Guid EventId { get; } = Guid.NewGuid();
        public DateTime Timestamp { get; } = DateTime.UtcNow;
        public string CorrelationId { get; set; }
        public Guid UserId { get; set; }
    }
}
