using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetPro.Core.Infrastructure;
using NetPro.TypeFinder;
using NetPro.Web.Core.Infrastructure.Extensions;
using Serilog;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

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
            application.UseSerilogRequestLogging();
            //异常处理
            application.UseNetProExceptionHandler();
            //404处理
            application.UsePageNotFound();
            //400处理
            application.UseStatusCodePages(context =>
            {
                //handle 400 (Bad request)
                if (context.HttpContext.Response.StatusCode == StatusCodes.Status400BadRequest)
                {
                    var logger = application.ApplicationServices.GetRequiredService<Microsoft.Extensions.Logging.ILogger<dynamic>>();
                    logger.LogError($"Error 400. Bad request,{context.HttpContext.Request.Path.Value}");
                }

                return Task.CompletedTask;
            });
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
