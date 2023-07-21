using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetPro.Pulsar
{
    public static class PulsarConfigExtensions
    {
        /// <summary>
        /// 注册 pulsar
        /// </summary>
        public static IServiceCollection AddPulsarConfig(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<PulsarOption>(configuration.GetSection(nameof(PulsarOption)));
            return services;
        }
    }
}
