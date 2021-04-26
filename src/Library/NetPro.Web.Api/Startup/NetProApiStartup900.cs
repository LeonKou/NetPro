using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetPro.Core.Configuration;
using NetPro.Core.Infrastructure;
using NetPro.Swagger;
using NetPro.TypeFinder;
using NetPro.Utility.Helpers;
using NetPro.Web.Api.Filters;
using Serilog;

namespace NetPro.Web.Api
{
    /// <summary>
    /// 配置应用程序启动时MVC需要的中间件
    /// </summary>
    public class NetProApiStartup900 : INetProStartup
    {
        /// <summary>
        /// Add and configure any of the middleware
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="configuration">Configuration root of the application</param>
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration, ITypeFinder typeFinder)
        {
        }

        public void Configure(IApplicationBuilder application)
        {
            //TODO流量分析等其他中间件 
            application.UseAuthorization();
        }

        /// <summary>
        /// Gets order of this startup configuration implementation
        /// </summary>
        public int Order
        {
            //MVC should be loaded last
            get { return 900; }
        }
    }
}
