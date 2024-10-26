using Application.Common.Models;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text;

namespace Api.Middlewares
{
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestResponseLoggingMiddleware> _logger;
        private readonly IStringLocalizer<object> _localization;
        public RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger, IStringLocalizer<object> localization)
        {
            _next = next;
            _logger = logger;
            _localization = localization;
        }

        public async Task Invoke(HttpContext context)
        {
            context.Request.EnableBuffering();

            var requestBodyContent = await ReadRequestBody(context.Request);
            var requestHeaders = FormatHeaders(context.Request.Headers);

            var originalResponseBodyStream = context.Response.Body;
            await using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            var stopwatch = Stopwatch.StartNew();
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                var elapsedMilliseconds = stopwatch.ElapsedMilliseconds;

                _logger.LogError(ex, "HTTP Request Information: {Method} {Path} {Headers} {Body} responded {StatusCode} in {ElapsedMilliseconds} ms",
                    context.Request.Method, context.Request.Path, requestHeaders, requestBodyContent, context.Response.StatusCode, elapsedMilliseconds);

                context.Response.StatusCode = StatusCodes.Status200OK;
                context.Response.ContentType = "application/json";

                var errorResponse = new ApiResponse<object>
                {
                    ErrorCode = ErrorCodes.InternalServerError,
                    ErrorMessage = _localization[ErrorCodes.InternalServerError]
                };

                var errorJson = JsonConvert.SerializeObject(errorResponse);
                await context.Response.WriteAsync(errorJson);
                return;
            }
            finally
            {
                stopwatch.Stop();

                var responseBodyContent = await ReadResponseBody(context.Response);
                var responseHeaders = FormatHeaders(context.Response.Headers);
                var elapsedMilliseconds = stopwatch.ElapsedMilliseconds;

                _logger.LogInformation("HTTP Request Information: {Method} {Path} {Headers} {Body} responded {StatusCode} in {ElapsedMilliseconds} ms",
                    context.Request.Method, context.Request.Path, requestHeaders, requestBodyContent, context.Response.StatusCode, elapsedMilliseconds);

                await responseBody.CopyToAsync(originalResponseBodyStream);
            }
        }

        private async Task<string> ReadRequestBody(HttpRequest request)
        {
            request.Body.Position = 0;
            var buffer = new byte[Convert.ToInt32(request.ContentLength)];
            await request.Body.ReadAsync(buffer, 0, buffer.Length);
            var bodyAsText = Encoding.UTF8.GetString(buffer);
            request.Body.Position = 0;
            return bodyAsText;
        }

        private async Task<string> ReadResponseBody(HttpResponse response)
        {
            response.Body.Position = 0;
            var buffer = new byte[response.Body.Length];
            await response.Body.ReadAsync(buffer, 0, buffer.Length);
            var bodyAsText = Encoding.UTF8.GetString(buffer);
            response.Body.Position = 0;
            return bodyAsText;
        }

        private static Dictionary<string, string> FormatHeaders(IHeaderDictionary headers)
        {
            return headers.ToDictionary(h => h.Key, h => h.Value.ToString());
        }
    }




}
