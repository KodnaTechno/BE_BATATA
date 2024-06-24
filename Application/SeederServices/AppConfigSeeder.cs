using AppCommon.DTOs;
using Consul;
using Infrastructure.Database.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Database.Configration
{
    public class AppConfigSeeder
    {
        private readonly ApplicationDbContext _context;
        private readonly IConsulClient _consulClient;

        public AppConfigSeeder(ApplicationDbContext context, IConsulClient consulClient)
        {
            _context = context;
            _consulClient = consulClient;
        }

        public async Task SeedAsync()
        {
            var defaultSettings = new Dictionary<string, object>
            {
                ["TimezoneSetting"] = new TimezoneSetting
                {
                    Region = new Region
                    {
                        Id = "DefaultRegion",
                        BaseUtcOffset = "00:00",
                        StandardName = "UTC",
                        DisplayName = "Coordinated Universal Time",
                        DaylightName = "",
                        Name = "UTC"
                    },
                    DateFormat = "yyyy-MM-dd",
                    TimeFormat = "hh:mm tt"
                },
                // Add more settings here
            };

            foreach (var setting in defaultSettings)
            {
                if (!await _context.AppConfigs.AnyAsync(ac => ac.Key == setting.Key))
                {
                    var config = new AppConfig
                    {
                        Key = setting.Key,
                        Value = JsonConvert.SerializeObject(setting.Value),
                        LastUpdated = DateTime.UtcNow,
                    };

                    await _context.AppConfigs.AddAsync(config);

                    // Consul configuration update
                    var consulResponse = await _consulClient.KV.Put(new KVPair($"config/app/{setting.Key}")
                    {
                        Value = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(setting.Value))
                    });

                    if (!consulResponse.Response)
                    {
                        throw new Exception($"Failed to write {setting.Key} to Consul");
                    }
                }
            }

            await _context.SaveChangesAsync();
        }
    }


}
