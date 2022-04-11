using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NetPro.CsRedis;
using System.NetPro;

namespace NetPro.Analysic
{
    internal sealed class NetProAnalysicStartup400 : INetProStartup, System.NetPro.Startup.__._
    {
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

        public void Configure(IApplicationBuilder application, IWebHostEnvironment env)
        {
            if (enabled)
                application.UseRequestAnalysis();
        }

        /// <summary>
        /// Gets order of this startup configuration implementation
        /// </summary>
        public double Order { get; set; } = 400;
    }
}
