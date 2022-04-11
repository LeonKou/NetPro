using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetPro.ZeroMQ;
using System.NetPro;

namespace NetPro.Taos
{
    /// <summary>
    /// ZeroMQStartup
    /// </summary>
    internal sealed class ZeroMQStartup : INetProStartup, System.NetPro.Startup.__._
    {
        /// <summary>
        /// 
        /// </summary>
        public double Order { get; set; } = 0;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <param name="typeFinder"></param>
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration = null, ITypeFinder typeFinder = null)
        {
            var option= new ZeroMQOption(configuration);
            services.AddSingleton(option);
            services.AddZeroMQForPublisher(option);
            services.AddZeroMQForPushSocket(option);
            services.AddZeroMQForResponseSocket(option);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="application"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder application, IWebHostEnvironment env)
        {
        }

    }
}
