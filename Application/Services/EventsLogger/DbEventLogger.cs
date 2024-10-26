using Events;
using Infrastructure.Database;
using Infrastructure.Database.Domain;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Text;

namespace Application.Services.EventsLogger
{
    public class DbEventLogger : IEventLogger
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public DbEventLogger(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task LogEventAsync(IEvent @event, CancellationToken cancellationToken = default)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var eventLog = new EventLog
            {
                Id = @event.EventId,
                EventType = @event.GetType().AssemblyQualifiedName,
                EventData = JsonConvert.SerializeObject(@event),
                OccurredOn = @event.Timestamp,
                CorrelationId = @event.CorrelationId,
                UserId = @event.UserId,
                Status = "Queued",
            };
            dbContext.EventLogs.Add(eventLog);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateEventStatusAsync(
            Guid eventId,
            string status,
            Exception exception = null,
            CancellationToken cancellationToken = default)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var eventLog = await dbContext.EventLogs.FindAsync(
                [eventId], cancellationToken);

            if (eventLog != null)
            {
                eventLog.Status = status;
                if (exception != null)
                {
                    var errorMessage = new StringBuilder();
                    errorMessage.AppendLine(exception.Message);
                    var currentException = exception.InnerException;
                    while (currentException != null)
                    {
                        errorMessage.AppendLine($"Inner Exception: {currentException.Message}");
                        currentException = currentException.InnerException;
                    }
                    eventLog.ErrorMessage = errorMessage.ToString();
                }
                else
                {
                    eventLog.ErrorMessage = null;
                }
                await dbContext.SaveChangesAsync(cancellationToken);
            }
        }

    }
}
