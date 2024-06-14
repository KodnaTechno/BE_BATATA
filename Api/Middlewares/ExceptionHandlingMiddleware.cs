using Microsoft.AspNetCore.Http;
using Serilog;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Api.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
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
            var request = context.Request;
            request.EnableBuffering();

            string bodyAsText = await new StreamReader(request.Body).ReadToEndAsync();
            request.Body.Position = 0;

            var requestInformation = new
            {
                Method = request.Method,
                Path = request.Path,
                QueryString = request.QueryString.ToString(),
                BodyContent = bodyAsText
            };

            //if (exception is CustomApplicationException customException)
            //{
            //    Log.ForContext("Type", "CustomException")
            //       .ForContext("RequestDetails", requestInformation, destructureObjects: true)
            //       .Error(customException, "A custom exception occurred.");
            //}
            //else
            //{
            //    Log.ForContext("Type", "UnhandledException")
            //       .ForContext("RequestDetails", requestInformation, destructureObjects: true)
            //       .Error(exception, "An unhandled exception occurred.");
            //}

            Log.ForContext("Type", "UnhandledException")
                  .ForContext("RequestDetails", requestInformation, destructureObjects: true)
                  .Error(exception, "An unhandled exception occurred.");


            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var result = JsonSerializer.Serialize(new { error = exception.Message });
            await context.Response.WriteAsync(result);
        }
    }

}
