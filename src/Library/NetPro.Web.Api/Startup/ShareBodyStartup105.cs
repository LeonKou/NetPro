using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetPro.Core.Infrastructure;
using NetPro.ShareRequestBody;
using NetPro.TypeFinder;

namespace NetPro.Web.Core.Infrastructure
{
    /// <summary>
    /// 共享body
    /// </summary>
    public class ShareBodyStartup105 : INetProStartup
    {
        /// <summary>
        /// 添加 
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="configuration">Configuration root of the application</param>
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration, ITypeFinder typeFinder)
        {
            services.AddShareRequestBody();
        }

        /// <summary>
        /// 添加要使用的中间件
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public void Configure(IApplicationBuilder application)
        {
            application.UseShareRequestBody();
        }

        /// <summary>
        /// Gets order of this startup configuration implementation
        /// </summary>
        public int Order
        {
            //authentication should be loaded before MVC
            get { return 105; }
        }
    }
}
