using Hangfire.Dashboard;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Hangfire.Shared
{
    public class HangfireDashboardAuthOptions
    {
        public string Username { get; set; } = "admin";
        public string Password { get; set; } = "admin";
        public bool RequireHttps { get; set; } = true;
        public string[] AllowedIPs { get; set; } = Array.Empty<string>();
    }

    public static class HangfireDashboardExtensions
    {
        public static IApplicationBuilder UseCustomHangfireDashboard(
            this IApplicationBuilder app,
            IConfiguration configuration,
            string route = "/hangfire")
        {
            var options = new DashboardOptions
            {
                Authorization = new[]
                {
                    new HangfireAuthorizationFilter(configuration)
                }
            };

            return app.UseHangfireDashboard(route, options);
        }
    }

    public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        private readonly HangfireDashboardAuthOptions _options;
        private readonly IConfiguration _configuration;

        public HangfireAuthorizationFilter(IConfiguration configuration)
        {
            _configuration = configuration;
            _options = configuration.GetSection("Hangfire:Dashboard").Get<HangfireDashboardAuthOptions>()
                ?? new HangfireDashboardAuthOptions();
        }

        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();

            // Check HTTPS requirement
            if (_options.RequireHttps && !httpContext.Request.IsHttps)
            {
                return false;
            }

            // Check IP whitelist
            if (_options.AllowedIPs.Length > 0)
            {
                var clientIp = httpContext.Connection.RemoteIpAddress?.ToString();
                if (clientIp != null && !_options.AllowedIPs.Contains(clientIp))
                {
                    return false;
                }
            }

            // Check Basic Authentication
            string authHeader = httpContext.Request.Headers["Authorization"].ToString();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Basic "))
            {
                SetAuthenticationChallenge(httpContext);
                return false;
            }

            try
            {
                var encodedCredentials = authHeader.Substring("Basic ".Length).Trim();
                var credentials = System.Text.Encoding.UTF8.GetString(
                    Convert.FromBase64String(encodedCredentials)).Split(':');

                var username = credentials[0];
                var password = credentials[1];

                // Fixed the syntax error (removed asterisks)
                if (username == _options.Username && password == _options.Password)
                {
                    return true;
                }
            }
            catch
            {
                // Invalid authorization header format
            }

            SetAuthenticationChallenge(httpContext);
            return false;
        }

        private void SetAuthenticationChallenge(HttpContext httpContext)
        {
            httpContext.Response.Headers["WWW-Authenticate"] = "Basic realm=\"Hangfire Dashboard\"";
            httpContext.Response.StatusCode = 401;
        }
    }
}