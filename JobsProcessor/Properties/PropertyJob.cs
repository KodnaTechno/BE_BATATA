using Events.Modules.Properties;
using Infrastructure.Caching;
using Microsoft.Extensions.Logging;
using Module;

namespace JobsProcessor.Properties
{
    public class PropertyJob : IPropertyJob
    {
        private readonly ILogger<PropertyJob> _logger;
        private readonly ModuleDbContext _moduleDbContext;
        private readonly IEntityCacheService _cacheService;

        public PropertyJob(ILogger<PropertyJob> logger, ModuleDbContext moduleDbContext, IEntityCacheService cacheService)
        {
            _logger = logger;
            _moduleDbContext = moduleDbContext;
            _cacheService = cacheService;
        }

        public void ProcessPropertyCreatedEvent(PropertyCreatedEvent @event)
        {
            _logger.LogInformation($"Processing PropertyCreatedEvent for property ID: {@event.Id}");

            _cacheService.RemoveByPrefixAsync("properties:")
            .GetAwaiter().GetResult();
        }

        public void ProcessPropertyUpdatedEvent(PropertyUpdatedEvent @event)
        {
            _logger.LogInformation($"Processing PropertyUpdatedEvent for property ID: {@event.Id}");

            _cacheService.RemoveByPrefixAsync("properties:")
            .GetAwaiter().GetResult();
        }

        public void ProcessPropertyDeletedEvent(PropertyDeletedEvent @event)
        {
            _logger.LogInformation($"Processing PropertyDeletedEvent for property ID: {@event.Id}");

            _cacheService.RemoveByPrefixAsync("properties:")
            .GetAwaiter().GetResult();
        }
    }
}
