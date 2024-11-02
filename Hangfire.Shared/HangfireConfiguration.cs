using Hangfire.SqlServer;
using Hangfire.MySql;
using Hangfire.PostgreSql;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace Hangfire.Shared
{
    public static class HangfireConfiguration
    {
        public static void AddHangfire(this IServiceCollection services, IConfiguration configuration, bool addHangfireServer = true)
        {
            var provider = configuration["DatabaseProvider"]?.ToLower() ?? "sqlserver";
            var connectionString = configuration.GetConnectionString("HangfireConnection");

            if (string.IsNullOrEmpty(connectionString))
                throw new InvalidOperationException("Hangfire connection string is not configured.");

            services.AddHangfire(config =>
            {
                config.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                    .UseSimpleAssemblyNameTypeSerializer()
                    .UseRecommendedSerializerSettings();

                switch (provider)
                {
                    case "sqlserver":
                        ConfigureSqlServer(config, connectionString);
                        break;

                    case "mysql":
                        ConfigureMySQL(config, connectionString);
                        break;

                    case "postgresql":
                        ConfigurePostgreSQL(config, connectionString);
                        break;

                    default:
                        ConfigureSqlServer(config, connectionString);
                        break;
                }
            });

            // Add the Hangfire server
            if (addHangfireServer)
            {
                services.AddHangfireServer(options =>
                {
                    options.WorkerCount = configuration.GetValue("Hangfire:WorkerCount", 6);
                });
            }
        }

        private static void ConfigureSqlServer(IGlobalConfiguration config, string connectionString)
        {
            config.UseSqlServerStorage(connectionString, new SqlServerStorageOptions
            {
                // Polling interval for checking queued jobs
                QueuePollInterval = TimeSpan.FromSeconds(15),

                // Expiration for completed jobs, to automatically delete after this period
                JobExpirationCheckInterval = TimeSpan.FromHours(1), // Check every hour for expired jobs

                // Optimizes performance by grouping multiple commands into a single batch
                CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),

                // Timeout for executing SQL commands
                CommandTimeout = TimeSpan.FromMinutes(2),

                // Configures the database schema for Hangfire
                SchemaName = "hangfire",

                // Configure job list limit for the Hangfire Dashboard
                DashboardJobListLimit = 10000,

                // Aggregates counters, reducing database load by grouping data operations
                CountersAggregateInterval = TimeSpan.FromMinutes(5),

                // Enables necessary schema creation if not already present
                PrepareSchemaIfNecessary = true,

                // Ensures that completed jobs are acknowledged in SQL Server within a transaction
                UseTransactionalAcknowledge = true,

                // Use fine-grained locking for better concurrency
                UseFineGrainedLocks = true,

                // Setting recommended isolation level to avoid deadlocks
                UseRecommendedIsolationLevel = true,

                // Enable automatic detection of schema-dependent options, optimizing configuration based on schema version
                TryAutoDetectSchemaDependentOptions = true,

                // Limits the number of records deleted in a single batch to prevent overload
                DeleteExpiredBatchSize = 1000,

                // Disables global locks for improved performance in highly concurrent environments
                DisableGlobalLocks = true
            });
        }

        private static void ConfigureMySQL(IGlobalConfiguration config, string connectionString)
        {
            config.UseStorage(new MySqlStorage(connectionString, new MySqlStorageOptions
            {
                // Frequency with which Hangfire polls the queue for new jobs
                QueuePollInterval = TimeSpan.FromSeconds(15),

                // Set interval for checking job expiration to avoid excessive data retention
                JobExpirationCheckInterval = TimeSpan.FromHours(1), // Check every hour for expired jobs

                // Aggregate counters at regular intervals to optimize database writes
                CountersAggregateInterval = TimeSpan.FromMinutes(5),

                // Automatically prepares the schema if not present, useful for first-time setup
                PrepareSchemaIfNecessary = true,

                // Limits the number of jobs displayed in the Hangfire Dashboard to avoid overload
                DashboardJobListLimit = 10000,

                // Timeout for transactions to ensure they don't lock up resources for too long
                TransactionTimeout = TimeSpan.FromMinutes(1),

                // Prefix for Hangfire tables, useful when sharing a database
                TablesPrefix = "Hangfire"
            }));
        }

        private static void ConfigurePostgreSQL(IGlobalConfiguration config, string connectionString)
        {
            config.UsePostgreSqlStorage(options =>
            {
                options.UseNpgsqlConnection(connectionString);

            }, new PostgreSqlStorageOptions
            {
                // Frequency of polling the queue for new jobs, balancing performance and responsiveness
                QueuePollInterval = TimeSpan.FromSeconds(15),

                // Interval for checking expired jobs to avoid excessive data accumulation
                JobExpirationCheckInterval = TimeSpan.FromHours(1), // Check every hour

                // Aggregate counters periodically to reduce frequent database writes
                CountersAggregateInterval = TimeSpan.FromMinutes(5),

                // Invisibility timeout for jobs - useful for long-running jobs
                InvisibilityTimeout = TimeSpan.FromMinutes(30),

                // Time duration for locking resources in distributed environments
                DistributedLockTimeout = TimeSpan.FromMinutes(10),

                // Number of records deleted per batch when removing expired jobs
                DeleteExpiredBatchSize = 1000,

                // Enable native PostgreSQL transactions for improved concurrency
                UseNativeDatabaseTransactions = true,

                // Ensure schema preparation if it's missing (useful for initial setup)
                PrepareSchemaIfNecessary = true,

                // Set schema name for Hangfire tables (useful when sharing a database)
                SchemaName = "hangfire",

                // Use sliding invisibility timeout if needed for long-running jobs
                UseSlidingInvisibilityTimeout = false
            });
        }
    }
}
