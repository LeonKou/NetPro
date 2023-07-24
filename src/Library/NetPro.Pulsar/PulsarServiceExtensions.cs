using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Pulsar.Client.Api;
using System;

namespace NetPro.Pulsar
{
    public static class PulsarServiceExtensions
    {
        /// <summary>
        /// 注册 pulsar
        /// </summary>
        public static IServiceCollection AddPulsarClient(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            services.Configure<PulsarOption>(configuration.GetSection("PulsarOption"));

            services.AddSingleton(sp =>
            {
                var pulsarOption = sp.GetRequiredService<IOptions<PulsarOption>>()?.Value;
                if (pulsarOption == null)
                    throw new ArgumentNullException(nameof(pulsarOption));

                return new PulsarClientBuilder().OperationTimeout(TimeSpan.FromSeconds(15))
                .ServiceUrl(pulsarOption.ServiceUrl)
                //.Authentication(AuthenticationFactory.Token(pulsarOption.Authentication?.Token ?? ""))
                .Authentication(new AuthenticationBasic(pulsarOption.Authentication?.UserName ?? "", pulsarOption.Authentication?.Password ?? ""))
                .BuildAsync().Result;

            });
            services.AddHttpApi<IPulsarAdminApi>(p => { p.HttpHost = new Uri($"{configuration.GetValue<string>("PulsarOption:AdminServiceUrl")}"); });
            services.AddSingleton<IPulsarQueneService, PulsarQueneService>();
            return services;
        }
    }
}
