using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using System.Globalization;
using System.Reflection;

namespace Localization
{
    public class JsonStringLocalizer : IStringLocalizer
    {
        private readonly IDistributedCache _cache;
        private readonly JsonSerializer _serializer = new();
        private readonly SemaphoreSlim _semaphore = new(1, 1);


        public JsonStringLocalizer(IDistributedCache cache)
        {
            _cache = cache;
        }


        public LocalizedString this[string name] => GetLocalizedString(name, arguments: null);

        public LocalizedString this[string name, params object[] arguments] => GetLocalizedString(name, arguments);

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            var filePath = $"Resources/{Thread.CurrentThread.CurrentCulture.Name}.json";

            using FileStream stream = new(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            using StreamReader streamReader = new(stream);
            using JsonTextReader reader = new(streamReader);

            while (reader.Read())
            {
                if (reader.TokenType != JsonToken.PropertyName)
                    continue;

                var key = reader.Value as string;
                reader.Read();
                var value = _serializer.Deserialize<string>(reader);
                yield return new LocalizedString(name: key, value: value);
            }
        }

        private LocalizedString GetLocalizedString(string name, object[] arguments)
        {
            try
            {
                var formatString = GetStringAsync(name).GetAwaiter().GetResult();
                var value = arguments == null ? formatString : string.Format(CultureInfo.CurrentCulture, formatString, arguments);
                return new LocalizedString(name, value);
            }
            catch (Exception ex)
            {
                return new LocalizedString(name, name, true);
            }
        }

        private async Task<string> GetStringAsync(string key)
        {
            string result = string.Empty;

            await _semaphore.WaitAsync(); 

            try
            {
                var cacheKey = $"locale_{Thread.CurrentThread.CurrentCulture.Name}_{key}";
                var cacheValue = _cache.GetString(cacheKey);

                if (!string.IsNullOrEmpty(cacheValue))
                {
                    return cacheValue;
                }

                var filePath = GetFilePath();

                if (File.Exists(filePath))
                {
                    result = await GetValueFromJSONAsync(key, filePath);

                    if (!string.IsNullOrEmpty(result))
                    {
                        _cache.SetString(cacheKey, result);
                    }
                }
            }
            catch (Exception ex)
            {
                //TODO: Log the exception
            }
            finally
            {
                _semaphore.Release();
            }

            return result;
        }

        private async Task<string> GetValueFromJSONAsync(string propertyName, string filePath)
        {
            try
            {
                using FileStream stream = new(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                using StreamReader streamReader = new(stream);
                using JsonTextReader reader = new(streamReader);

                while (await reader.ReadAsync())
                {
                    if (reader.TokenType == JsonToken.PropertyName && reader.Value as string == propertyName)
                    {
                        await reader.ReadAsync();
                        return _serializer.Deserialize<string>(reader);
                    }
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                // Log the exception
                return string.Empty;
            }
        }

        private string GetFilePath()
        {
            string assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var fileName = $"{CultureInfo.CurrentCulture.Name}.json";
            return Path.Combine(assemblyDirectory, "Resources", fileName);
        }
    }

}
