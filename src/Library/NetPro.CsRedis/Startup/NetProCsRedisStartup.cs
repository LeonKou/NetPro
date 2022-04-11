using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.NetPro;

namespace NetPro.CsRedis
{
    internal sealed class NetProCsRedisStartup : INetProStartup, System.NetPro.Startup.__._
    {
        public double Order { get; set; } = 1000;

        public void Configure(IApplicationBuilder application, IWebHostEnvironment env)
        {
        }

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration = null, ITypeFinder typeFinder = null)
        {
            //新增redis缓存注入
            services.AddCsRedis<SystemTextJsonSerializer>(configuration);
        }
    }
}
