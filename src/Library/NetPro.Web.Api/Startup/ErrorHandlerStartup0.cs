using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetPro.Core.Infrastructure;
using NetPro.TypeFinder;
using NetPro.Web.Core.Infrastructure.Extensions;

namespace NetPro.Web.Core.Infrastructure
{
    /// <summary>
    /// 配置应用程序启动时异常处理中间件
    /// </summary>
    public class ErrorHandlerStartup : INetProStartup
    {
        /// <summary>
        /// Add and configure any of the middleware
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="configuration">Configuration root of the application</param>
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration, ITypeFinder typeFinder)
        {
        }

        /// <summary>
        /// Configure the using of added middleware
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public void Configure(IApplicationBuilder application)
        {
            //异常处理
            application.UseNetProExceptionHandler();
            //400处理
            application.UseBadRequestResult();
            //404处理
            application.UsePageNotFound();
        }

        /// <summary>
        /// Gets order of this startup configuration implementation
        /// </summary>
        public int Order
        {
            //error handlers should be loaded first
            get { return 0; }
        }
    }
}
