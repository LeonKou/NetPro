using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using NetPro.Core.Configuration;
using NetPro.Core.Infrastructure;
using NetPro.RedisManager;
using NetPro.TypeFinder;
using NetPro.Web.Core.Compression;
using System.Linq;

namespace NetPro.Web.Core.Infrastructure
{
    /// <summary>
    /// 配置应用程序启动时常用的中间件
    /// </summary>
    public class NetProCommonStartup : INetProStartup
    {
        /// <summary>
        /// Add and configure any of the middleware
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="configuration">Configuration root of the application</param>
        [System.Obsolete]
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration, ITypeFinder typeFinder)
        {
            //compression
            services.AddResponseCompression();

            //add options feature
            services.AddOptions();

            //新增redis缓存注入
            if (configuration.GetValue<bool>("RedisCacheOption:Enabled", false))
                services.AddRedisManager(configuration);

            services.Scan(scan => scan
           .FromAssemblies(typeFinder.GetAssemblies().Where(s =>
                 s.GetName().Name.EndsWith("Repository")).ToArray()) //搜索Repository程序集
           .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Repository")))//搜索Repository结尾的类
           .AsImplementedInterfaces()
           .WithScopedLifetime());
        }
        /// <summary>
        /// Configure the using of added middleware
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public void Configure(IApplicationBuilder application)
        {
            var config = EngineContext.Current.Resolve<NetProOption>();

            //compression
            if (config.UseResponseCompression)
            {
                //gzip by default
                application.UseResponseCompression();
                //workaround with "vary" header
                application.UseMiddleware<ResponseCompressionVaryWorkaroundMiddleware>();
            }

            //static files
            application.UseStaticFiles(new StaticFileOptions
            {
                //TODO duplicated code (below)
                OnPrepareResponse = ctx =>
                {
                    if (!string.IsNullOrEmpty(config.StaticFilesCacheControl))
                        ctx.Context.Response.Headers.Append(HeaderNames.CacheControl, config.StaticFilesCacheControl);
                }
            });
        }

        /// <summary>
        /// Gets order of this startup configuration implementation
        /// </summary>
        public int Order
        {
            //common services should be loaded after error handlers
            get { return 100; }
        }
    }
}
