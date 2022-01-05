using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NetPro.Checker;
using NetPro.Core.Configuration;
using NetPro.Core.Infrastructure;
using System.Collections.Generic;
using System.Net;
using System.NetPro;

namespace NetPro.Checker
{
    public class CheckerStartup : INetProStartup, System.NetPro.Startup.__._
    {
        /// <summary>
        /// Add and configure any of the middleware
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="configuration">Configuration root of the application</param>
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration, ITypeFinder typeFinder)
        {
            var healthbuild = services.AddHealthChecks();
            //services.AddHealthChecksUI(setupSettings: setup =>
            //{
            //    setup.SetApiMaxActiveRequests(1);
            //    //Only one active request will be executed at a time.
            //    //All the excedent requests will result in 429 (Too many requests)
            //});
            //var logger = services.BuildServiceProvider().GetService<ILogger<NetProCheckerStartup400>>();

            //logger.LogInformation($"[{nameof(NetProCheckerStartup400)}] 未打开");
        }

        public void Configure(IApplicationBuilder application, IWebHostEnvironment env)
        {
            //var config = EngineContext.Current.Resolve<NetProOption>();

            //if (!config.EnabledHealthCheck) return;

            application.UseHealthChecks("/health", new HealthCheckOptions()
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            //application.UseHealthChecksUI(s => s.UIPath = "/hc-ui");

            application.UseCheck();
        }

        /// <summary>
        /// Gets order of this startup configuration implementation
        /// </summary>
        public double Order { get; set; } = 0;
    }
}
