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
                options.UseSqlServer(AppConfigration.AppDbConnection, sqlOptions =>
                {
                    sqlOptions.MigrationsAssembly("AppMigration.SqlServer");
                    sqlOptions.MigrationsHistoryTable("__AppIdentity_MigrationTable");
                });
            });


            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(AppConfigration.IdentityDbConnection, sqlOptions =>
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

            //builder.Services.AddDistributedMemoryCache(); // Adds a default in-memory implementation of IDistributedCache
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout
                options.Cookie.HttpOnly = true; // Set HttpOnly for security
                options.Cookie.IsEssential = true; // Make the session cookie essential
            });
            //builder.Services.AddHttpContextAccessor();
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
}
