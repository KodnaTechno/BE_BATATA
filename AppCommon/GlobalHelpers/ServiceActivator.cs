using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCommon.GlobalHelpers
{
    public static class ServiceActivator
    {
        private static IServiceProvider _serviceProvider;
        private static object _lock = new();

        public static void Configure(IServiceProvider serviceProvider)
        {
            if (_serviceProvider != null)
                throw new InvalidOperationException("ServiceActivator is already configured.");

            lock (_lock)
            {
                if (_serviceProvider != null)
                    throw new InvalidOperationException("ServiceActivator is already configured.");

                _serviceProvider = serviceProvider;
            }
        }

        public static IServiceProvider GetServiceProvider()
        {
            if (_serviceProvider == null)
                throw new InvalidOperationException("ServiceActivator is not configured. Call Configure first.");

            return _serviceProvider;
        }

        public static IServiceScope GetScope()
        {
            return GetServiceProvider().CreateScope();
        }

        public static T GetService<T>() where T : class
        {
            if (_serviceProvider == null)
                throw new InvalidOperationException("ServiceActivator is not configured. Call Configure first.");

            return _serviceProvider.GetService<T>();
        }

        public static T GetRequiredService<T>() where T : class
        {
            if (_serviceProvider == null)
                throw new InvalidOperationException("ServiceActivator is not configured. Call Configure first.");

            return _serviceProvider.GetRequiredService<T>();
        }

        public static T GetScopedService<T>() where T : class
        {
            using var scope = GetScope();
            return scope.ServiceProvider.GetRequiredService<T>();
        }
    }
}
