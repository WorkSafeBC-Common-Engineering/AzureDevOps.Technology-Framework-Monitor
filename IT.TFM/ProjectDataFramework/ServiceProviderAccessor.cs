using System;
using System.Threading;

using Microsoft.Extensions.DependencyInjection;

namespace ProjectData
{
    public static class ServiceProviderAccessor
    {
        private static IServiceProvider _serviceProvider;

        public static void Initialize(IServiceProvider serviceProvider)
        {
            ArgumentNullException.ThrowIfNull(serviceProvider);
            Interlocked.Exchange(ref _serviceProvider, serviceProvider);
        }

        public static T GetRequiredService<T>() where T : notnull
        {
            return GetServiceProvider().GetRequiredService<T>();
        }

        public static T GetRequiredKeyedService<T>(object serviceKey) where T : notnull
        {
            return GetServiceProvider().GetRequiredKeyedService<T>(serviceKey);
        }

        private static IServiceProvider GetServiceProvider()
        {
            // TODO: Remove this bridge when callers can use injected dependencies instead of static factories.
            return Volatile.Read(ref _serviceProvider)
                ?? throw new InvalidOperationException("The application service provider has not been initialized.");
        }
    }
}
