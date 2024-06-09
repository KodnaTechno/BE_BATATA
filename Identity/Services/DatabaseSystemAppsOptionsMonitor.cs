using System;
using System.Collections.Generic;
using System.Threading;
using AppIdentity.Database;
using AppIdentity.Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
namespace AppIdentity.Services
{
  
        public class DatabaseSystemAppsOptionsMonitor : IOptionsMonitor<SystemAppOptions>, IDisposable
        {
            private readonly IServiceProvider _serviceProvider;
            private readonly Timer _timer;
            private SystemAppOptions _currentValue;

            public DatabaseSystemAppsOptionsMonitor(IServiceProvider serviceProvider, TimeSpan pollingInterval)
            {
                 _serviceProvider = serviceProvider;
                _timer = new Timer(RefreshOptions, null, TimeSpan.Zero, pollingInterval);
                _currentValue = GetCurrentOptions();
            }

            public SystemAppOptions CurrentValue => _currentValue;

            public SystemAppOptions Get(string name)
            {
                return _currentValue;
            }

          

            private void RefreshOptions(object state)
            {
                SystemAppOptions newOptions = GetCurrentOptions();

                if (!EqualityComparer<SystemAppOptions>.Default.Equals(newOptions, _currentValue))
                {
                    _currentValue = newOptions;
                    // Trigger the change event if options have changed
                    OnChange?.Invoke(newOptions, "");
                }
            }

            private SystemAppOptions GetCurrentOptions()
            {
                using var scope = _serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetService<AppIdentityDbContext>();
                var appCredentials = context?.AppCredentials.ToList() ?? new List<AppCredential>();

                var options = new SystemAppOptions();
                options.RegisteredApps = appCredentials;

                return options;
            }

            public void Dispose()
            {
                _timer.Dispose();
            }

        IDisposable IOptionsMonitor<SystemAppOptions>.OnChange(Action<SystemAppOptions, string> listener)
        {
            return default;
        }

        public event Action<SystemAppOptions, string> OnChange;
        }
}
