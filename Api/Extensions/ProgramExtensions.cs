using AppCommon.DTOs;
using AppCommon;
using Infrastructure.Database;
using Localization.ServiceFactory;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Module.ServiceFactory;
using AppIdentity;
using Infrastructure.Database.Configration;
using AppCommon.GlobalHelpers;
using Consul;
using Import.ServiceFactory;
using Application;
using Infrastructure.DependencyInjection;
using Hangfire;
using Hangfire.SqlServer;
using System.Reflection;
using Api.Jobs;
using Serilog;
using Winton.Extensions.Configuration.Consul;
using FileStorge;

namespace Api.Extensions
{
    public static class ProgramExtensions
    {
        public static void AddCustomServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddCulture();
            AppConfigration.Configure(configuration);
            services.AddModule(configuration);
            services.AddAppIdentity(configuration, options =>
            {
                options.UseSqlServer(AppConfigration.IdentityDbConnection, sqlOptions =>
                {
                    sqlOptions.MigrationsAssembly("AppMigration.SqlServer");
                    sqlOptions.MigrationsHistoryTable("__AppIdentity_MigrationTable");
                });
            });


            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(AppConfigration.AppDbConnection, sqlOptions =>
                {
                    sqlOptions.MigrationsAssembly("AppMigration.SqlServer");
                    sqlOptions.MigrationsHistoryTable("__App_MigrationTable");
                });
            });

            services.Configure<TimezoneSetting>(configuration.GetSection("timezoneSetting"));


            services.AddSingleton<IConsulClient, ConsulClient>(p => new ConsulClient(consulConfig =>
            {
                consulConfig.Address = new Uri(configuration["Consul:Host"]);
            }));

            services.AddScoped<AppConfigSeeder>();

            services.AddImportServices();

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout
                options.Cookie.HttpOnly = true; // Set HttpOnly for security
                options.Cookie.IsEssential = true; // Make the session cookie essential
            });

            services.AddHttpContextAccessor();
            services.AddApplication();

            services.AddFlexibleCaching(configuration);

            services.AddHangfire(configuration);


            services.AddFileProvider(configuration);
        }

        public static void ApplyMigrations(this IApplicationBuilder app, params Type[] dbContexts)
        {
            using var scope = app.ApplicationServices.CreateScope();
            foreach (var dbContextType in dbContexts)
            {
                var dbContext = (DbContext)scope.ServiceProvider.GetRequiredService(dbContextType);
                dbContext.Database.Migrate();
            }
        }

        public static async Task SeedDatabaseAsync(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var seeder = scope.ServiceProvider.GetRequiredService<AppConfigSeeder>();
            await seeder.SeedAsync();
        }


        public static void ConfigureTimezoneSettings(this WebApplication app)
        {
            var timezoneSettings = app.Services.GetRequiredService<IOptionsMonitor<TimezoneSetting>>();
            DateTimeExtensions.ConfigureTimezone(timezoneSettings);
        }
    }




    public static class HangfireConfiguration
    {
        public static void AddHangfire(this IServiceCollection services, IConfiguration configuration)
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
                        throw new ArgumentException($"Unsupported database provider: {provider}");
                }
            });

            // Add the Hangfire server
            services.AddHangfireServer(options =>
            {
                options.WorkerCount = configuration.GetValue<int>("Hangfire:WorkerCount", 1);
            });
        }

        private static void ConfigureSqlServer(IGlobalConfiguration config, string connectionString)
        {
            config.UseSqlServerStorage(connectionString, new SqlServerStorageOptions
            {
                CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                QueuePollInterval = TimeSpan.FromSeconds(15),
                UseRecommendedIsolationLevel = true,
                DisableGlobalLocks = true
            });
        }

        private static void ConfigureMySQL(IGlobalConfiguration config, string connectionString)
        {
            //config.UseStorage(new MySqlStorage(connectionString, new MySqlStorageOptions
            //{
            //    TablesPrefix = "Hangfire",
            //    QueuePollInterval = TimeSpan.FromSeconds(15),
            //    JobExpirationCheckInterval = TimeSpan.FromHours(1),
            //    CountersAggregateInterval = TimeSpan.FromMinutes(5),
            //    PrepareSchemaIfNecessary = true,
            //    DashboardJobListLimit = 50000,
            //    TransactionTimeout = TimeSpan.FromMinutes(1),
            //    QueuesCleaner = true
            //}));
        }

        private static void ConfigurePostgreSQL(IGlobalConfiguration config, string connectionString)
        {
            //config.UsePostgreSqlStorage(connectionString, new PostgreSqlStorageOptions
            //{
            //    QueuePollInterval = TimeSpan.FromSeconds(15),
            //    SchemaName = "hangfire"
            //});
        }

        public static void ActivateHangfireJobs()
        {
            var baseJobType = typeof(BaseJob);
            var jobTypes = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && baseJobType.IsAssignableFrom(t));

            foreach (var jobType in jobTypes)
            {
                var jobInstance = Activator.CreateInstance(jobType) as BaseJob;
                jobInstance?.Define();
            }
        }
    }



    public static class SerilogExtensions
    {
        public static WebApplicationBuilder AddSerilogConfiguration(this WebApplicationBuilder builder)
        {
            builder.Host.UseSerilog((hostingContext, services, loggerConfiguration) =>
            {
                loggerConfiguration
                    .ReadFrom.Configuration(hostingContext.Configuration)
                    .Enrich.FromLogContext();
            });

            return builder;
        }
    }


    public static class ConsulExtensions
    {
        public static WebApplicationBuilder AddConsulConfiguration(this WebApplicationBuilder builder)
        {
            var consulHost = builder.Configuration["Consul:Host"];

            try
            {
                builder.Configuration.AddConsul("config/app", options =>
                {
                    options.ConsulConfigurationOptions = cco =>
                    {
                        cco.Address = new Uri(consulHost);
                    };
                    options.Optional = true;
                    options.ReloadOnChange = true;
                    options.PollWaitTime = TimeSpan.FromSeconds(5);
                });

                builder.Services.AddSingleton<IConsulClient>(p => new ConsulClient(cfg =>
                {
                    cfg.Address = new Uri(consulHost);
                }));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to configure Consul: {ex.Message}");
            }
            return builder;
        }
    }



}
