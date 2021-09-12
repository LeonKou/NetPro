using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NetPro.Analysic;
using NetPro.Core.Infrastructure;
using NetPro.CsRedis;
using NetPro.TypeFinder;

namespace NetPro.Analysic
{
    public class NetProAnalysicStartup400 : INetProStartup
    {
        public string Description => $"{this.GetType().Namespace} 支持请求分析，风控";
        private static bool enabled = true;
        /// <summary>
        /// Add and configure any of the middleware
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="configuration">Configuration root of the application</param>
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration, ITypeFinder typeFinder)
        {
            //redis没开启，流量监控无法打开
            var redisCacheOption = configuration.GetSection(nameof(RedisCacheOption)).Get<RedisCacheOption>();
            var logger = services.BuildServiceProvider().GetService<ILogger<NetProAnalysicStartup400>>();
            if (redisCacheOption != null && redisCacheOption.Enabled)
            {
                services.AddRequestAnalysic();
            }
            else
            {
                enabled = false;
                logger.LogInformation($"[{nameof(NetProAnalysicStartup400)}] 未打开");
            }
        }

        public void Configure(IApplicationBuilder application)
        {
            if (enabled)
                application.UseRequestAnalysis();
        }

        /// <summary>
        /// Gets order of this startup configuration implementation
        /// </summary>
        public int Order
        {
            get { return 400; }
        }
    }
}
