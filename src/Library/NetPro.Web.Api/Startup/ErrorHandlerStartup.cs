using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
//using Serilog;
using System.NetPro;
using System.Threading.Tasks;

namespace NetPro.Web.Api
{
    /// <summary>
    /// 配置应用程序启动时异常处理中间件
    /// app.UseExceptionHandler()
    /// </summary>
    internal sealed class ErrorHandlerStartup : INetProStartup, System.NetPro.Startup.__._
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
        public void Configure(IApplicationBuilder application, IWebHostEnvironment env)
        {
            //application.UseSerilogRequestLogging();//<PackageReference Include="Serilog.AspNetCore" Version="4.1.0" />
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
        public double Order { get; set; } = 0;  //error handlers should be loaded first
    }
}
