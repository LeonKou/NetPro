using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetPro.Core.Infrastructure;
using NetPro.TypeFinder;

namespace NetPro.CsRedis
{
    public class NetProCsRedisStartup1000 : INetProStartup
    {
        public int Order => 1000;

        public string Description => $"{this.GetType().Namespace} 支持CsRedis注入";

        public void Configure(IApplicationBuilder application)
        {
        }

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration = null, ITypeFinder typeFinder = null)
        {
            //新增redis缓存注入
            if (configuration.GetValue<bool>("RedisCacheOption:Enabled", false))
                services.AddCsRedis<SystemTextJsonSerializer>(configuration);
        }
    }
}
