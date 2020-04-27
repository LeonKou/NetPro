using Microsoft.Extensions.DependencyInjection;
using System;

namespace MQMiddleware
{
    /// <summary>
    /// A service extension for registration exchange singleton "services".
    /// </summary>
    internal class ExchangeServiceDescriptor : ServiceDescriptor
    {
        public string ExchangeName { get; set; }

        public ExchangeServiceDescriptor(Type serviceType, object instance)
            : base(serviceType, instance)
        {
        }

        public ExchangeServiceDescriptor(Type serviceType, Type implementationType, ServiceLifetime lifetime)
            : base(serviceType, implementationType, lifetime)
        {
        }

        public ExchangeServiceDescriptor(Type serviceType, Func<IServiceProvider, object> factory, ServiceLifetime lifetime)
            : base(serviceType, factory, lifetime)
        {
        }
    }
}