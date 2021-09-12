using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using NetPro.Core.Configuration;
using NetPro.Core.Infrastructure;
using NetPro.CsRedis;
using NetPro.TypeFinder;
using NetPro.Web.Core.Compression;
using System.Linq;

namespace NetPro.Web.Api
{
    /// <summary>
    /// 文件中间件
    /// </summary>
    public class NetProStaticFilesStartup100 : INetProStartup
    {
        public string Description => $"{this.GetType().Namespace} 支持 UseStaticFiles；UseResponseCompression响应压缩";
        /// <summary>
        /// Add and configure any of the middleware
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="configuration">Configuration root of the application</param>
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration, ITypeFinder typeFinder)
        {
            //compression
            services.AddResponseCompression();

            //add options feature
            services.AddOptions();

           // //批量注入Repository
           // services.Scan(scan => scan
           //.FromAssemblies(typeFinder.GetAssemblies().Where(s =>
           //      s.GetName().Name.EndsWith("Repository")).ToArray()) //搜索Repository程序集
           //.AddClasses(classes => classes.Where(type => type.Name.EndsWith("Repository")))//搜索Repository结尾的类
           //.AsImplementedInterfaces()
           //.WithScopedLifetime());
        }

        /// <summary>
        /// Configure the using of added middleware
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public void Configure(IApplicationBuilder application)
        {
            var config = EngineContext.Current.Resolve<NetProOption>();
            //UseRouting之前的中间件
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
