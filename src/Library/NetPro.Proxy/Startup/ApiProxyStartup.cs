using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetPro.Core.Infrastructure;
using NetPro.TypeFinder;
using NetPro.Proxy;
using Microsoft.AspNetCore.Hosting;

namespace NetPro.Proxy
{
    /// <summary>
    /// Proxy远程请求组件
    /// MicroServicesEndpoint:Assembly配置当前Proxy接口所在程序集名称
    /// </summary>
    public class ApiProxyStartup : INetProStartup
    {
        public double Order { get; set; } = 0;

        public void Configure(IApplicationBuilder application, IWebHostEnvironment env)
        {
        }

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration = null, ITypeFinder typeFinder = null)
        {
            services.AddHttpProxy(configuration, typeFinder, configuration.GetValue<string>("NetProProxyOption:Assembly", string.Empty));
        }
    }
}
