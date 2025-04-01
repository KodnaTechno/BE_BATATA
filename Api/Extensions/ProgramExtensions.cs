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
using Microsoft.OpenApi.Models;

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
            
            services.AddDbContext<WorkflowDbContext>(options =>
            {
                options.UseSqlServer(AppConfigration.AppDbConnection, sqlOptions =>
                {
                    sqlOptions.MigrationsAssembly("AppMigration.SqlServer");
                    sqlOptions.MigrationsHistoryTable("__AppWorkFlow_MigrationTable");
                });
            });

            services.Configure<TimezoneSetting>(configuration.GetSection("timezoneSetting"));


            services.AddSingleton<IConsulClient, ConsulClient>(p => new ConsulClient(consulConfig =>
            {
                consulConfig.Address = new Uri(configuration["Consul:Host"]);
            }));

            services.AddScoped<AppConfigSeeder>();

            services.AddScoped<ApplicationManager>();

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

            services.AddFileProvider(configuration);

            services.AddWorkflowInfrastructure(configuration);
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
