using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using NetPro.Core.Configuration;
using NetPro.Core.Infrastructure;
using NetPro.Web.Core.Compression;
using NetPro.RedisManager;
using System.Linq;
using NetPro.TypeFinder;

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
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration, ITypeFinder typeFinder)
        {
            //compression
            services.AddResponseCompression();

            //add options feature
            services.AddOptions();

            //新增redis缓存注入
            if (configuration.GetValue<bool>("RedisCacheOption:Enabled", false))
                services.AddRedisManager(configuration);

            //MongoDb 连接配置文件
            //if (configuration.GetValue<bool>("MongoDbOptions:Enabled", false))
            //{
            //    services.ConfigureStartupConfig<MongoDbOptions>(configuration.GetSection("MongoDbOptions"));
            //    var mongoDbOptions = services.BuildServiceProvider().GetRequiredService<MongoDbOptions>();
            //    if (string.IsNullOrWhiteSpace(mongoDbOptions?.ConnectionString))
            //    {
            //        return;
            //    }

            //    services.AddMongoDb(options =>
            //    {
            //        options = mongoDbOptions;
            //    });
            //}

            //services = services.AddDapperRepository();
            services.Scan(scan => scan
               .FromAssemblies(typeFinder.GetAssemblies().Where(s => s.GetName().Name.EndsWith("Repository")).ToArray()
                   .Where(s => s.GetName().Name.EndsWith("Repository")))
               .AddClasses()
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
