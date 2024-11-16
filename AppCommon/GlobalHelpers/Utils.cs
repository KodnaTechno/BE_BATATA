using System.Collections.Concurrent;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.Scripting.Hosting;
using Microsoft.VisualBasic;
using Newtonsoft.Json;

namespace AppCommon.GlobalHelpers
{
    public class ScriptGlobals
    {
        public Dictionary<string, string> Props { get; set; }
        public static int GetDays(TimeSpan t) => t.Days;
        public static int GetCurrentYear() => DateTime.UtcNow.Date.Year;
        public static int GetYear(DateTime date) => date.Year;
        public static int GetCurrentMonth() => DateTime.UtcNow.Date.Month;
        public static int GetMonth(DateTime date) => date.Month;
        public static int GetMonthsInDuration(TimeSpan span) => span.Days / 30;
        public static DateTime GetDateNow() => DateTime.UtcNow.Date;

        public DateTime SafeConvertToDateTime(string dateString)
        {
            if (string.IsNullOrEmpty(dateString))
            {
                throw new ArgumentNullException(nameof(dateString), "Date string cannot be null or empty.");
            }

            if (DateTime.TryParse(dateString, out DateTime result))
            {
                return result;
            }
            else
            {
                throw new FormatException($"'{dateString}' is not a valid date string.");
            }
        }

    }
    public static class Utils
    {
        private static readonly ConcurrentDictionary<string,  Script<object>> ScriptCache = new();
        public static async Task<object> ExecuteFormulaWithResult(string script, Dictionary<string, string> props, object defaultValue)
        {
            try
            {
                if (string.IsNullOrEmpty(script)) return defaultValue;

                var scriptBuilder = new StringBuilder(script);

                // Replace placeholders with dictionary access expressions
                foreach (var prop in props)
                {
                    string keyPattern = $@"@{prop.Key}";
                    scriptBuilder = scriptBuilder.Replace(keyPattern, $"Props[\"{prop.Key}\"]");
                }
                scriptBuilder
                          .Replace("CommonHelper.GetDateNow", "GetDateNow()")
                          .Replace("CommonHelper.", "")
                          .Replace("System.DateTime.Now", "GetDateNow()")
                          .Replace("Abs", "System.Math.Abs")
                          .Replace("\"Props[\"", "Props[\"")
                          .Replace("\"]\"", "\"]")
                          .Replace("System.Convert.ToDateTime", "SafeConvertToDateTime")
                          .Replace("Convert.ToDateTime", "SafeConvertToDateTime");


                var str = scriptBuilder.ToString();

                // Check if the script is already compiled and cached
                if (!ScriptCache.TryGetValue(str, out var compiledScript))
                {
                    // Compile the script and cache it
                    compiledScript = CSharpScript.Create<object>(
                        str,
                        options: ScriptOptions.Default.WithReferences(AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.IsDynamic && !string.IsNullOrEmpty(a.Location))),
                        globalsType: typeof(ScriptGlobals), assemblyLoader: new InteractiveAssemblyLoader());

                    ScriptCache.TryAdd(str, compiledScript);
                }

                // Prepare the globals object with the properties
                var globals = new ScriptGlobals { Props = props };

                // Run the cached script
                var result = await compiledScript.RunAsync(globals);
                if (result.ReturnValue is double doubleResult)
                {
                    if (double.IsInfinity(doubleResult) || double.IsNaN(doubleResult)) return 0;
                }
                return result.ReturnValue ?? defaultValue;
            }
            catch (Exception ex)
            {
                return defaultValue;
            }
        }
       
    }
}
