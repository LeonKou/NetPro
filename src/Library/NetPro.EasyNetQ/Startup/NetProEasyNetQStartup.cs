using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.NetPro;

namespace NetPro.EasyNetQ
{
    /// <summary>
    /// 
    /// </summary>
    internal sealed class NetProEasyNetQStartup : INetProStartup, System.NetPro.Startup.__._
    {
        public double Order { get; set; } = int.MaxValue;

        public void Configure(IApplicationBuilder application, IWebHostEnvironment env)
        {
        }

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration, ITypeFinder typeFinder)
        {
            services.AddEasyNetQ(configuration);
        }
    }
}
