using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetPro.Authentication;
using NetPro.Core.Infrastructure;
using NetPro.TypeFinder;

namespace NetPro.Web.Core.Infrastructure
{
    /// <summary>
    ///配置应用程序启动时身份验证中间件
    /// </summary>
    public class AuthenticationStartup : INetProStartup
    {
        /// <summary>
        /// 添加 处理身份认证服务相关的中间件实现
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="configuration">Configuration root of the application</param>
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration, ITypeFinder typeFinder)
        {
            services.AddNetProAuthentication(configuration);
        }

        /// <summary>
        /// 添加要使用的中间件
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public void Configure(IApplicationBuilder application)
        {
            application.UseNetProAuthentication();
        }

        /// <summary>
        /// Gets order of this startup configuration implementation
        /// </summary>
        public int Order
        {
            //authentication should be loaded before MVC
            get { return 500; }
        }
    }
}
