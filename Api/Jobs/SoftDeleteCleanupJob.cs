using Microsoft.EntityFrameworkCore;
using Module.Domain.Base;
using Module;
using AppCommon.GlobalHelpers;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Api.Jobs
{
    public class SoftDeleteCleanupJob : BaseJob
    {
        protected override string JobName => nameof(SoftDeleteCleanupJob);
        // Run at 3 AM on the 1st day of every month
        protected override string CronExpression => "0 3 1 * *";

        public override void Execute()
        {
            using var scope = ServiceActivator.GetScope();
            var logger = (ILogger<SoftDeleteCleanupJob>)scope.ServiceProvider.GetService(typeof(ILogger<SoftDeleteCleanupJob>));
            var moduleDbContext = (ModuleDbContext)scope.ServiceProvider.GetService(typeof(ModuleDbContext));

            try
            {
                var entityTypes = moduleDbContext.Model.GetEntityTypes()
                    .Where(t => typeof(BaseEntity).IsAssignableFrom(t.ClrType));

                foreach (var entityType in entityTypes)
                {
                    try
                    {
                        ProcessModuleEntityType(moduleDbContext, entityType, logger);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex,
                            "Error cleaning up soft-deleted records for entity {Entity}",
                            entityType.ClrType.Name);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while cleaning up soft-deleted records");
            }
        }

        private void ProcessModuleEntityType(ModuleDbContext dbContext, IEntityType entityType, ILogger<SoftDeleteCleanupJob> logger)
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-30);
            var method = typeof(DbContext)
                .GetMethod(nameof(DbContext.Set))
                ?.MakeGenericMethod(entityType.ClrType);

            if (method == null)
            {
                logger.LogError("Could not find Set method for entity type {Entity}", entityType.ClrType.Name);
                return;
            }

            var dbSet = method.Invoke(dbContext, null) as IQueryable<BaseEntity>;
            if (dbSet == null)
            {
                logger.LogError("Could not create DbSet for entity type {Entity}", entityType.ClrType.Name);
                return;
            }

            var query = dbSet
                .IgnoreQueryFilters()
                .Where(e => e.IsDeleted && e.DeletedAt <= cutoffDate);

            const int batchSize = 1000;
            var totalDeleted = 0;

            while (true)
            {
                var batch = query.Take(batchSize).ToList();
                if (batch.Count == 0)
                    break;

                foreach (var entity in batch)
                {
                    dbContext.Entry(entity).State = EntityState.Deleted;
                }

                totalDeleted += batch.Count;
                dbContext.SaveChanges();
            }

            logger.LogInformation(
                "Cleaned up {Count} soft-deleted records from {Entity}",
                totalDeleted,
                entityType.ClrType.Name);
        }
    }
}