using System;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using AppIdentity.Database;
using AppIdentity.Domain;
using AppIdentity.IServices;
using AppIdentity.Services;
using AppIdentity.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;
using AppCommon;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication.Cookies;
namespace AppIdentity
{
    public static class IdentityContext
    {

        public static void AddAppIdentity(this IServiceCollection services, IConfiguration configuration, Action<DbContextOptionsBuilder> dbContextConfigurator)
        {
            ConfigureIdentity(services, dbContextConfigurator);
            AddAuthenticationServices(services, configuration);
           
        }
        public static void UseAppIdentity(this IApplicationBuilder app)
        {
            // Ensure the authentication middleware is added to the pipeline
            app.UseAuthentication();
            app.UseAuthorization();

        }
        public static void MigrateIdentitySchema(this IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope();
            var context = serviceScope.ServiceProvider.GetRequiredService<AppIdentityDbContext>();
            context.Database.Migrate();
        }

        private static void ConfigureIdentity(IServiceCollection services, Action<DbContextOptionsBuilder> dbContextConfigurator)
        {
            services.AddIdentity<AppUser, AppRole>(options =>
            {
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;
            })
            .AddEntityFrameworkStores<AppIdentityDbContext>()
            .AddDefaultTokenProviders();

            services.AddDbContext<AppIdentityDbContext>(dbContextConfigurator);
            RegisterCommonServices(services);
        }

        private static void RegisterCommonServices(IServiceCollection services)
        {
            services.AddScoped<IUserProvider, AppUserProvider>();
            services.AddScoped<IRoleProvider, RoleProvider>();
            services.AddScoped<IGroupProvider, GroupProvider>();
            services.AddScoped<IAccessibilityProvider, AccessibilityProvider>();
            services.AddTransient<IAuthorizationHandler, AccessibilityRequirementHandler>();
            services.AddTransient<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
        }
        private static void AddAuthenticationServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(options =>
            {
                // Set the default scheme to JWT to handle API authentication
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme; // This is typically used for interactive applications
            })
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.RequireHttpsMetadata = true;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AppConfigration.JWTKey)), // Securely manage the secret key
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            // Conditional addition of Google authentication
            var googleSettings = configuration.GetSection("Authentication:Google");
            if (googleSettings.GetValue<bool>("Enabled"))
            {
                services.AddAuthentication()
                    .AddGoogle(googleOptions =>
                    {
                        googleOptions.ClientId = googleSettings["ClientId"];
                        googleOptions.ClientSecret = googleSettings["ClientSecret"];
                    });
            }

            // Add more providers as needed
        }
        
    }
}
