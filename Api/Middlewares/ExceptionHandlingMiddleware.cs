using System.Text;
using AppCommon.DTOs;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;

namespace Api.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly IStringLocalizer<object> _localization;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger, IStringLocalizer<object> localization)
        {
            _next = next;
            _logger = logger;
            _localization = localization;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var requestBodyContent = await ReadRequestBody(context.Request);
            var requestHeaders = context.Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());

            _logger.LogError(exception, "An unhandled exception has occurred while executing the request. " +
                "Request Information: {Method} {Path} {Headers} {Body}",
                context.Request.Method, context.Request.Path, requestHeaders, requestBodyContent);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status200OK;

            var response = new ApiResponse<object>
            {
                ErrorCode = ErrorCodes.InternalServerError,
                ErrorMessage = _localization[ErrorCodes.InternalServerError]
            };

            var responseBody = JsonConvert.SerializeObject(response);
            await context.Response.WriteAsync(responseBody);
        }

        private async Task<string> ReadRequestBody(HttpRequest request)
        {
            request.Body.Position = 0;
            using var reader = new StreamReader(request.Body, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true);
            var bodyAsText = await reader.ReadToEndAsync();
            request.Body.Position = 0;
            return bodyAsText;
        }
    }

    public static class ExceptionHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionHandlingMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }
}

