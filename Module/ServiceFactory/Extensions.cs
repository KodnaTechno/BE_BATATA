using AppCommon;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
namespace Module.ServiceFactory
{
    public static class Extensions
    {
        public static void AddModule(this IServiceCollection services, IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("DefaultConnection");
            string databaseProvider = configuration["DatabaseProvider"];

          
            switch (databaseProvider)
            {
                case "SqlServer":
                    ConfigureSqlServerDbContext(services, connectionString);
                    break;
                case "PostgreSQL":
                    //ConfigurePostgreSQLDbContext(services, connectionString);
                    break;
                default:
                    throw new Exception("Invalid database provider");
            }
        }

        public static void ConfigureSqlServerDbContext(IServiceCollection services, string connectionString)
        {
            services.AddDbContext<ModuleDbContext>(options =>
            {
                options.UseSqlServer(connectionString, sqlOptions =>
                {
                    sqlOptions.MigrationsAssembly("AppMigration.SqlServer");
                    sqlOptions.MigrationsHistoryTable("__Module_MigrationTable");
                });
            });
        

        }
     
        //public static void ConfigurePostgreSQLDbContext(IServiceCollection services, string connectionString)
        //{
        //    services.AddDbContext<ModuleDbContext>(options =>
        //    {
        //        options.UseNpgsql(connectionString);
        //    });
        //}
    }

}
