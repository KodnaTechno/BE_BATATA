using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System.Globalization;

namespace Localization.ServiceFactory
{
    public static class Extensions
    {
        public static void AddCulture(this IServiceCollection services)
        {
            services.AddLocalization();
            services.AddDistributedMemoryCache();
            services.AddSingleton<IStringLocalizerFactory, JsonStringLocalizerFactory>();

            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[]
                {
                    new CultureInfo("en-US"),
                    new CultureInfo("ar-EG"),
                };

                options.DefaultRequestCulture = new RequestCulture(culture: supportedCultures[0]);
                options.SupportedCultures = supportedCultures;
            });
        }

        public static IApplicationBuilder UseCultureMiddleware(this IApplicationBuilder builder)
        {
            var supportedCultures = new[] { "en-US", "ar-EG" };
            var defaultCulture = "en-US";

            return builder.Use(next =>
            {
                var middleware = new CultureMiddleware(next, supportedCultures, defaultCulture);
                return async ctx =>
                {
                    await middleware.InvokeAsync(ctx);
                };
            });
        }
        public static void UseCulture(this IApplicationBuilder app)
        {
            app.UseCultureMiddleware();

            var supportedCultures = new[] { "en-US", "ar-EG" };

            var localizationOptions = new RequestLocalizationOptions()
                .SetDefaultCulture(supportedCultures[0])
                .AddSupportedCultures(supportedCultures);

            app.UseRequestLocalization(localizationOptions);
        }

    }
}
