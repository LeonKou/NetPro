using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using NetPro.Web.Api;
using System.Net;

namespace XXX.API.Startup
{
    /// <summary>
    /// 自定义异常处理中间件
    /// </summary>
    [ReplaceStartup("ErrorHandlerStartup")]
    public class CustomErrorHandlerStartup : INetProStartup
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
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder application, IWebHostEnvironment env)
        {
            application.UseExceptionHandler(handler =>
            {
                handler.Run(async context =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "text/plain; charset=utf-8";
                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (contextFeature != null)
                    {
                        if (contextFeature.Error != null)
                        {
                            var exError = contextFeature.Error;
                            await context.Response.WriteAsync($"Error has been catch by CustomErrorHandlerStartup:\r\nMessage: {exError.Message}\r\nStackTrace: {exError.StackTrace}");
                            return;
                        }
                    };
                });
            });
        }

        /// <summary>
        /// Gets order of this startup configuration implementation
        /// </summary>
        public double Order { get; set; } = 0;  //error handlers should be loaded first
    }
}
