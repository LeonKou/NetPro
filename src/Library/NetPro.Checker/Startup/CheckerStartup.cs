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
        private NetProCheckerOption _mqtttClientOptions;
        /// <summary>
        /// Add and configure any of the middleware
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="configuration">Configuration root of the application</param>
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration, ITypeFinder typeFinder)
        {
            var mqtttClientOptions = new NetProCheckerOption(configuration);
            _mqtttClientOptions = mqtttClientOptions;
            if (mqtttClientOptions.Enabled)
            {
                var healthbuild = services.AddHealthChecks();
            }

            //services.AddHealthChecksUI(setupSettings: setup =>
            //{
            //    setup.SetApiMaxActiveRequests(1);
            //    //Only one active request will be executed at a time.
            //    //All the excedent requests will result in 429 (Too many requests)
            //});
            //var logger = services.BuildServiceProvider().GetService<ILogger<CheckerStartup>>();

            //logger.LogInformation($"[{nameof(CheckerStartup)}] 未打开");
        }

        public void Configure(IApplicationBuilder application, IWebHostEnvironment env)
        {
            //var config = EngineContext.Current.Resolve<NetProOption>();

            //if (!config.EnabledHealthCheck) return;

            //application.UseHealthChecks("/health", new HealthCheckOptions()
            //{
            //    Predicate = _ => true,
            //    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            //});

            //application.UseHealthChecksUI(s => s.UIPath = "/hc-ui");
            if (_mqtttClientOptions.Enabled)
            {
                if (!_mqtttClientOptions.EnvPath.StartsWith("/"))
                {
                    _mqtttClientOptions.EnvPath = "/"+_mqtttClientOptions.EnvPath;
                }
                application.UseEnvCheck(_mqtttClientOptions.EnvPath);
                if (!_mqtttClientOptions.InfoPath.StartsWith("/"))
                {
                    _mqtttClientOptions.InfoPath = "/" + _mqtttClientOptions.InfoPath;
                }
                application.UseInfoCheck(_mqtttClientOptions.InfoPath);
                if (!_mqtttClientOptions.HealthPath.StartsWith("/"))
                {
                    _mqtttClientOptions.HealthPath = "/" + _mqtttClientOptions.HealthPath;
                }
                application.UseHealthCheck(_mqtttClientOptions.HealthPath);
            }

            //application.UseCheck();
        }

        /// <summary>
        /// Gets order of this startup configuration implementation
        /// </summary>
        public double Order { get; set; } = 0;
    }
}
