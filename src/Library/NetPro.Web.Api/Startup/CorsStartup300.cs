using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NetPro.Core.Configuration;
using NetPro.Core.Infrastructure;
using NetPro.TypeFinder;

namespace NetPro.Web.Api
{
    /// <summary>
    /// 跨域
    /// application.UseCors();
    /// </summary>
    class CorsStartup300 : INetProStartup
    {
        /// <summary>
        /// Add and configure any of the middleware
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="configuration">Configuration root of the application</param>
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration, ITypeFinder typeFinder)
        {
            var serviceProvider = services.BuildServiceProvider();
            var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
            ILogger logger = null;
            if (loggerFactory != null)
            {
                logger = loggerFactory.CreateLogger($"{nameof(CorsStartup300)}");
            }

            var netProOption = serviceProvider.GetService<NetProOption>();
            var corsOrigins = ConvertHelper.ToList<string>(netProOption.CorsOrigins).ToArray();
            //支持跨域访问
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", (builder) =>
                {
                    if (!string.IsNullOrEmpty(netProOption.CorsOrigins) && netProOption.CorsOrigins == "*")//所有域名都可跨域
                    {
                        builder = builder.SetIsOriginAllowed((host) => true);
                        logger?.LogInformation($"全局跨域已开启");
                    }
                    else
                    {
                        builder = builder.WithOrigins(corsOrigins);
                        logger?.LogInformation($"指定域名{string.Join(',', corsOrigins)}跨域已开启");
                    }
                    builder.AllowAnyMethod()
                       .AllowAnyHeader()
                       //.WithMethods("GET", "POST")
                       .AllowCredentials();
                });
            });
        }

        public void Configure(IApplicationBuilder application)
        {
            application.UseCors("CorsPolicy");
        }

        /// <summary>
        /// Gets order of this startup configuration implementation
        /// </summary>
        public double Order
        {
            //MVC should be loaded last
            get { return 300; }
        }
    }
}
