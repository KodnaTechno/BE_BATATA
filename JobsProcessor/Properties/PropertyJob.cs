using Events.Modules.Properties;
using Microsoft.Extensions.Logging;
using Module;

namespace JobsProcessor.Properties
{
    public class PropertyJob : IPropertyJob
    {
        private readonly ILogger<PropertyJob> _logger;
        private readonly ModuleDbContext _moduleDbContext;

        public PropertyJob(ILogger<PropertyJob> logger, ModuleDbContext moduleDbContext)
        {
            _logger = logger;
            _moduleDbContext = moduleDbContext;
        }

        public void ProcessPropertyCreatedEvent(PropertyCreatedEvent @event)
        {
            _logger.LogInformation($"Processing PropertyCreatedEvent for property ID: {@event.Id}");

            // Here you can implement additional logic when a property is created
            // For example, you might need to create related entities or 
            // perform additional setup steps
        }

        public void ProcessPropertyUpdatedEvent(PropertyUpdatedEvent @event)
        {
            _logger.LogInformation($"Processing PropertyUpdatedEvent for property ID: {@event.Id}");
        }

        public void ProcessPropertyDeletedEvent(PropertyDeletedEvent @event)
        {
            _logger.LogInformation($"Processing PropertyDeletedEvent for property ID: {@event.Id}");
        }
    }
}
