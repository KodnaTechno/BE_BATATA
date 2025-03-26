namespace Infrastructure.Database.Domain
{
    public class EventLog
    {
        public Guid Id { get; set; }
        public string EventType { get; set; }
        public string EventData { get; set; }
        public DateTime OccurredOn { get; set; }
        public string CorrelationId { get; set; }
        public string UserId { get; set; }
        public string Status { get; set; } // Queued, Processing, Processed, Failed
        public string ErrorMessage { get; set; }
        public int RetryCount { get; set; }
    }

}
