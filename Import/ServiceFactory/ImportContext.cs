
using Import.IServices;
using Import.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Import.ServiceFactory
{
    public static class ImportContext
    {


        public static void AddImportServices(this IServiceCollection services)
        {
            services.AddScoped<IImportFromExcel, ImportFromExcel>();
            //services.AddDistributedMemoryCache();
            //services.AddSession(options =>
            //{
            //    options.IdleTimeout = TimeSpan.FromMinutes(30);
            //    options.Cookie.HttpOnly = true;
            //    options.Cookie.IsEssential = true;
            //});
            // Handle listOfCommands as required...
        }

    }
}
