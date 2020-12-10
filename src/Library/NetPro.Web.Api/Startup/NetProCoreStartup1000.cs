using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetPro.Checker;
using NetPro.Core.Configuration;
using NetPro.Core.Infrastructure;
using NetPro.RedisManager;
using NetPro.TypeFinder;
using NetPro.Web.Core.Infrastructure.Extensions;
using System.Collections.Generic;

namespace NetPro.Web.Core.Infrastructure
{
    /// <summary>
    /// 配置应用程序启动时MVC需要的中间件
    /// </summary>
    public class NetProCoreStartup : INetProStartup
    {
        /// <summary>
        /// Add and configure any of the middleware
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="configuration">Configuration root of the application</param>
        /// <param name="typeFinder"></param>
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration, ITypeFinder typeFinder)
        {
            var serviceProvider = services.BuildServiceProvider();
            //var redisCacheOption = serviceProvider.GetService<RedisCacheOption>();
            var netproOption = serviceProvider.GetService<NetProOption>();

            //配置 ef性能监控
            //services.AddMiniProfilerEF();
            //配置 mvc服务
            services.AddNetProCore(netproOption);


            //健康检查
            if (!netproOption?.EnabledHealthCheck??true)
                return;

            var healthbuild = services.AddHealthChecks();
            //if (!string.IsNullOrWhiteSpace(mongoDbOptions?.ConnectionString))
            //    healthbuild.AddMongoDb(mongoDbOptions.ConnectionString, tags: new string[] { "mongodb" });
            //foreach (var item in redisCacheOption?.Endpoints ?? new List<ServerEndPoint>())
            //{
            //    healthbuild.AddRedis($"{item.Host}:{item.Port},password={redisCacheOption.Password}", name: $"redis-{item.Host}:{item.Port}");
            //}

            //services.AddHealthChecksUI();
        }

        /// <summary>
        /// Configure the using of added middleware
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public void Configure(IApplicationBuilder application)
        {
            application.Use(next => context =>
            {
                context.Request.EnableBuffering();
                return next(context);
            });

            var config = EngineContext.Current.Resolve<NetProOption>();
            //if (config.MiniProfilerEnabled)
            //{
            //    //add MiniProfiler
            //    application.UseMiniProfiler();
            //    application.UseMiddleware<MiniProfilerMiddleware>();
            //}

            if (!config.EnabledHealthCheck) return;

            application.UseHealthChecks("/health", new HealthCheckOptions()
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            //application.UseHealthChecksUI(s => s.UIPath = "/ui");

            application.UseCheck();
        }

        /// <summary>
        /// Gets order of this startup configuration implementation
        /// </summary>
        public int Order => 1000;
    }
}
