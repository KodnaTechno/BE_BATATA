using AppCommon.DTOs;
using AppCommon;
using Infrastructure.Database;
using Localization.ServiceFactory;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Module.ServiceFactory;
using AppIdentity;
using AppCommon.GlobalHelpers;
using Consul;
using Import.ServiceFactory;
using Application;
using Infrastructure.DependencyInjection;
using Serilog;
using Winton.Extensions.Configuration.Consul;
using FileStorge;
using Events;
using Application.SeederServices;
using Application.Services;
using AppWorkflow.Infrastructure.Data.Context;
using AppWorkflow.Infrastructure;
using AppWorkflow.Core.Interfaces.Services;
using AppWorkflow.Infrastructure.Actions;
using Application.AppWorkflowActions;
using AppWorkflow.Infrastructure.Services.Actions;
using Microsoft.OpenApi.Models;

namespace Api.Extensions
{
    #region Program Extensions
    public static class ProgramExtensions
    {
        #region Add Custom Services
        public static void AddCustomServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Add culture and configuration
            services.AddCulture();
            AppConfigration.Configure(configuration);

            // Add modules and identity
            services.AddModule(configuration);
            ConfigureIdentity(services, configuration);

            // Add database contexts
            ConfigureDatabaseContexts(services);

            // Add configuration settings
            services.Configure<TimezoneSetting>(configuration.GetSection("timezoneSetting"));

            // Add Consul client
            ConfigureConsulClient(services, configuration);

            // Add application services
            services.AddScoped<AppConfigSeeder>();
            services.AddScoped<ApplicationManager>();
            services.AddImportServices();
            services.AddHttpContextAccessor();
            services.AddApplication();
            services.AddFlexibleCaching(configuration);
            services.AddFileProvider(configuration);

            // Add session configuration
            ConfigureSession(services);

            // Add workflow infrastructure
            ConfigureWorkflowInfrastructure(services, configuration);
        }
        #endregion

        #region Private Configuration Methods
        private static void ConfigureIdentity(IServiceCollection services, IConfiguration configuration)
        {
            services.AddAppIdentity(configuration, options =>
            {
                options.UseSqlServer(AppConfigration.IdentityDbConnection, sqlOptions =>
                {
                    sqlOptions.MigrationsAssembly("AppMigration.SqlServer");
                    sqlOptions.MigrationsHistoryTable("__AppIdentity_MigrationTable");
                });
            });
        }

        private static void ConfigureDatabaseContexts(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(AppConfigration.AppDbConnection, sqlOptions =>
                {
                    sqlOptions.MigrationsAssembly("AppMigration.SqlServer");
                    sqlOptions.MigrationsHistoryTable("__App_MigrationTable");
                });
            });

            services.AddDbContext<WorkflowDbContext>(options =>
            {
                options.UseSqlServer(AppConfigration.AppDbConnection, sqlOptions =>
                {
                    sqlOptions.MigrationsAssembly("AppMigration.SqlServer");
                    sqlOptions.MigrationsHistoryTable("__AppWorkFlow_MigrationTable");
                });
            });
        }

        private static void ConfigureConsulClient(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IConsulClient, ConsulClient>(p => new ConsulClient(consulConfig =>
            {
                consulConfig.Address = new Uri(configuration["Consul:Host"]);
            }));
        }

        private static void ConfigureSession(IServiceCollection services)
        {
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout
                options.Cookie.HttpOnly = true; // Set HttpOnly for security
                options.Cookie.IsEssential = true; // Make the session cookie essential
            });
        }

        private static void ConfigureWorkflowInfrastructure(IServiceCollection services, IConfiguration configuration)
        {
            var assembly = typeof(CreateModuleAction).Assembly;
            Type baseType = typeof(WorkflowActionBase);
            var derivedTypes = assembly.GetTypes()
                                       .Where(t => t.IsClass && !t.IsAbstract && baseType.IsAssignableFrom(t))
                                       .ToList();
            services.AddWorkflowInfrastructure(configuration, derivedTypes);
        }
        #endregion

        #region Application Methods
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
        #endregion
    }
    #endregion

    #region Serilog Extensions
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
    #endregion

    #region Consul Extensions
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
    #endregion
}