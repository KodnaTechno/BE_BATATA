using System.Globalization;

namespace Api.Middlewares
{

    public class CultureMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly HashSet<string> _supportedCultures;
        private readonly string _defaultCulture;

        public CultureMiddleware(RequestDelegate next)
        {
            _next = next;
            _supportedCultures = new HashSet<string>(["en-US", "ar-EG"]);
            _defaultCulture = "en-US";
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string cultureToSet = _defaultCulture; // Default
            var acceptLanguageHeader = context.Request.Headers["Accept-Language"].ToString();

            if (!string.IsNullOrEmpty(acceptLanguageHeader))
            {
                var culturesWithQuality = acceptLanguageHeader.Split(',')
                    .Select(StringWithQuality.Create)
                    .Where(s => _supportedCultures.Contains(s.String))
                    .OrderByDescending(s => s.Quality)
                    .ToList();

                if (culturesWithQuality.Count != 0)
                {
                    cultureToSet = culturesWithQuality.First().String;
                }
            }

            CultureInfo.CurrentCulture = new CultureInfo(cultureToSet);
            CultureInfo.CurrentUICulture = new CultureInfo(cultureToSet);

            await _next(context);
        }

        private class StringWithQuality
        {
            public string String { get; }
            public double Quality { get; }

            private StringWithQuality(string @string, double quality)
            {
                String = @string;
                Quality = quality;
            }

            public static StringWithQuality Create(string stringWithQuality)
            {
                var parts = stringWithQuality.Split(';');
                var @string = parts[0].Trim();
                var quality = 1.0;

                if (parts.Length > 1)
                {
                    var qValue = parts[1].Split('=')[1];
                    double.TryParse(qValue, out quality);
                }

                return new StringWithQuality(@string, quality);
            }
        }
    }


    public static class CultureMiddlewareExtensions
    {
        public static IApplicationBuilder UseCultureMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CultureMiddleware>();
        }
    }
}
