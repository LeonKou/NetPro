using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Consul.AspNetCore
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add default <see cref="IConsulClient"/>.
        /// First client can be accessed from DI, to create multiple named clients use <see cref="IConsulClientFactory"/>
        /// </summary>
        public static IServiceCollection AddConsul(this IServiceCollection services)
        {
            return services.AddConsul(options => { });
        }

        /// <summary>
        /// Add default <see cref="IConsulClient"/> with configured <see cref="IOptions{ConsulClientConfiguration}"/>.
        /// First client can be accessed from DI, to create multiple named clients use <see cref="IConsulClientFactory"/>
        /// </summary>
        public static IServiceCollection AddConsul(
            this IServiceCollection services,
            Action<ConsulClientConfiguration> configure)
        {
            return services.AddConsul(Options.DefaultName, configure);
        }

        /// <summary>
        /// Add named <see cref="IConsulClient"/> with configured <see cref="IOptions{ConsulClientConfiguration}"/>.
        /// First client can be accessed from DI, to create multiple named clients use <see cref="IConsulClientFactory"/>
        /// </summary>
        public static IServiceCollection AddConsul(
            this IServiceCollection services,
            string name,
            Action<ConsulClientConfiguration> configure)
        {
            services.Configure(name, configure);
            services.TryAddSingleton<IConsulClientFactory, ConsulClientFactory>();
            services.TryAddSingleton(sp => sp.GetRequiredService<IConsulClientFactory>().CreateClient(name));

            return services;
        }

        /// <summary>
        /// Register consul service with default and support only ttl <see cref="IConsulClient"/>.
        /// First client can be accessed from DI, to create multiple named clients use <see cref="IConsulClientFactory"/>
        /// </summary>
        public static IServiceCollection AddConsulServiceRegistration(
            this IServiceCollection services,
            Action<AgentServiceRegistration> configure)
        {
            var registration = new AgentServiceRegistration();

            configure.Invoke(registration);

            return services
                .AddSingleton(registration)
                .AddHostedService<AgentServiceRegistrationHostedService>();
        }

        public static IServiceCollection AddConsulCatalogRegistration(
           this IServiceCollection services,
           Action<CatalogRegistration> configure)
        {
            var registration = new CatalogRegistration();

            configure.Invoke(registration);

            return services
                .AddSingleton(registration)
                .AddHostedService<CatalogRegistrationHostedService>();
        }

        public static void UseConsuClient(this IApplicationBuilder application)
        {
            var feature = application.ServerFeatures.Get<IServerAddressesFeature>();
            var addresses = feature.Addresses;
            var ports = addresses.Select(address => BindingAddress.Parse(address).Port);
            foreach (var item in ports)
            {
                Console.WriteLine($"feature addresses is {item}");
            }
            AgentServiceRegistrationHostedService.PORT = ports.First();
            CatalogRegistrationHostedService.PORT = ports.First();
        }
    }

    public class AgentCheckRegistration_1_7 : AgentCheckRegistration
    {
        public string CheckID { get; set; }
    }
}
