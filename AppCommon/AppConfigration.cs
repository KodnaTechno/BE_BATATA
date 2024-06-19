using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCommon
{
    public static class AppConfigration
    {
        private static IConfiguration _configuration;

        // Static property to access JWT_KEY
        private static string Get(string key)
        {
            return _configuration[key];
        }
        public static T GetT<T>(string key)
        {
            return System.Text.Json.JsonSerializer.Deserialize<T>(_configuration[key] ?? throw new NotImplementedException("Key Not Found In App Settings."));
        }
        public static string JWTKey => _configuration["AppIdentity:JWT:Key"];
        public static string IdentityDbConnection => _configuration["AppIdentity:Db:Connection"];
        public static string AppDbConnection => _configuration["ConnectionStrings:DefaultConnection"];

        // Initialize the configuration statically (to be called from Startup.cs)
        public static void Configure(IConfiguration configuration)
        {
            _configuration = configuration;
        }
    }
}
