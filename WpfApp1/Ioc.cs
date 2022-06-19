// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace WpfApp1
{
    public sealed class Ioc : IServiceProvider
    {
        public static Ioc Default { get; } = new Ioc();

        private volatile IServiceProvider serviceProvider;

        public object GetService(Type serviceType)
        {
            //ArgumentNullException.ThrowIfNull(serviceType);

            IServiceProvider provider = this.serviceProvider;

            if (provider is null)
            {
                ThrowInvalidOperationExceptionForMissingInitialization();
            }

            return provider.GetService(serviceType);
        }

        public T GetService<T>()
            where T : class
        {
            IServiceProvider provider = this.serviceProvider;

            if (provider is null)
            {
                ThrowInvalidOperationExceptionForMissingInitialization();
            }

            return (T)provider.GetService(typeof(T));
        }

        public T GetRequiredService<T>()
            where T : class
        {
            IServiceProvider provider = this.serviceProvider;

            if (provider is null)
            {
                ThrowInvalidOperationExceptionForMissingInitialization();
            }

            T service = (T)provider.GetService(typeof(T));

            if (service is null)
            {
                ThrowInvalidOperationExceptionForUnregisteredType();
            }

            return service;
        }

        public void ConfigureServices(IServiceProvider serviceProvider)
        {
            //ArgumentNullException.ThrowIfNull(serviceProvider);

            IServiceProvider oldServices = Interlocked.CompareExchange(ref this.serviceProvider, serviceProvider, null);

            if (oldServices != null)
            {
                ThrowInvalidOperationExceptionForRepeatedConfiguration();
            }
        }

        private static void ThrowInvalidOperationExceptionForMissingInitialization()
        {
            throw new InvalidOperationException("The service provider has not been configured yet.");
        }

        private static void ThrowInvalidOperationExceptionForUnregisteredType()
        {
            throw new InvalidOperationException("The requested service type was not registered.");
        }

        private static void ThrowInvalidOperationExceptionForRepeatedConfiguration()
        {
            throw new InvalidOperationException("The default service provider has already been configured.");
        }
    }
}