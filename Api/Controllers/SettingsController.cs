using Consul;
using Infrastructure.Database;
using Infrastructure.Database.Domain;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SettingsController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IConsulClient _consulClient;

        public SettingsController(ApplicationDbContext dbContext, IConsulClient consulClient)
        {
            _dbContext = dbContext;
            _consulClient = consulClient;
        }

        [HttpGet("{key}")]
        public async Task<IActionResult> GetSetting(string key)
        {
            var setting = await _dbContext.AppConfigs.FindAsync(key);
            if (setting == null)
                return NotFound();

            return Ok(setting);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateSetting([FromBody] AppConfig updatedConfig)
        {
            var config = await _dbContext.AppConfigs.FindAsync(updatedConfig.Key);
            if (config == null)
            {
                return NotFound();
            }

            config.Value = updatedConfig.Value;
            config.LastUpdated = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();

            // Update the value in Consul
            await UpdateConsul(config.Key, config.Value);

            return Ok();
        }

        private async Task UpdateConsul(string key, string value)
        {
            var putPair = new KVPair($"config/app/{key}")
            {
                Value = Encoding.UTF8.GetBytes(value)
            };
            await _consulClient.KV.Put(putPair);
        }
    }
}
