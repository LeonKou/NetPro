using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.NetPro;

namespace NetPro.FreeRedis
{
    internal sealed class NetProFreeRedisStartup : INetProStartup, System.NetPro.Startup.__._
    {
        public double Order { get; set; } = 1000;

        public void Configure(IApplicationBuilder application, IWebHostEnvironment env)
        {
        }

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration = null, ITypeFinder typeFinder = null)
        {
            //新增redis缓存注入
            services.AddFreeRedis(configuration);
        }
    }
}
