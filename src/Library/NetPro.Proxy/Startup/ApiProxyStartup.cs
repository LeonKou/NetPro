using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.NetPro;

namespace NetPro.Proxy
{
    /// <summary>
    /// Proxy远程请求组件
    /// MicroServicesEndpoint:Assembly配置当前Proxy接口所在程序集名称
    /// </summary>
    internal sealed class ApiProxyStartup : INetProStartup, System.NetPro.Startup.__._
    {
        /// <summary>
        /// 
        /// </summary>
        public double Order { get; set; } = 0;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="application"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder application, IWebHostEnvironment env)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <param name="typeFinder"></param>
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration = null, ITypeFinder typeFinder = null)
        {
            services.AddHttpProxy(configuration, typeFinder,
                configuration.GetValue<string>($"{nameof(NetProProxyOption)}:AssemblyPattern", string.Empty),
                configuration.GetValue<string>($"{nameof(NetProProxyOption)}:InterfacePattern", string.Empty));
        }
    }

    /// <summary>
    /// 配置
    /// </summary>
    public class NetProProxyOption
    {
        /// <summary>
        /// 匹配程序集的正则
        /// </summary>
        public string AssemblyPattern { get; set; }

        /// <summary>
        /// 匹配接口的正则
        /// </summary>
        public string InterfacePattern { get; set; }

    }
}
