/*
 *  MIT License
 *  
 *  Copyright (c) 2021 LeonKou
 *  
 *  Permission is hereby granted, free of charge, to any person obtaining a copy
 *  of this software and associated documentation files (the "Software"), to deal
 *  in the Software without restriction, including without limitation the rights
 *  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 *  copies of the Software, and to permit persons to whom the Software is
 *  furnished to do so, subject to the following conditions:
 *  
 *  The above copyright notice and this permission notice shall be included in all
 *  copies or substantial portions of the Software.
 *  
 *  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 *  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 *  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 *  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 *  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 *  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 *  SOFTWARE.
 */

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.NetPro;

namespace NetPro.Checker
{
    internal sealed class CheckerStartup : INetProStartup, System.NetPro.Startup.__._
    {
        private NetProCheckerOption _checkerOption;
        /// <summary>
        /// Add and configure any of the middleware
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="configuration">Configuration root of the application</param>
        /// <param name="typeFinder"></param>
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration, ITypeFinder typeFinder)
        {
            var checkerOption = new NetProCheckerOption(configuration);
            _checkerOption = checkerOption;
            if (checkerOption.Enabled)
            {
                services.AddHealthChecks();
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
            //application.MapWhen(context =>
            //context.Request.Path == "/", (appBuilder) =>
            //{
            //    appBuilder.Run(async (context) =>
            //    {
            //        context.Response.StatusCode = 200;
            //        context.Response.ContentType = "text/html; charset=utf-8";
            //        await context.Response.WriteAsync("");
            //    });
            //});

            //var config = EngineContext.Current.Resolve<NetProOption>();

            //if (!config.EnabledHealthCheck) return;

            //application.UseHealthChecks("/health", new HealthCheckOptions()
            //{
            //    Predicate = _ => true,
            //    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            //});

            //application.UseHealthChecksUI(s => s.UIPath = "/hc-ui");
            if (_checkerOption.Enabled)
            {
                if (!_checkerOption.EnvPath.StartsWith("/"))
                {
                    _checkerOption.EnvPath = "/" + _checkerOption.EnvPath;
                }
                application.UseEnvCheck(_checkerOption.EnvPath);
                if (!_checkerOption.InfoPath.StartsWith("/"))
                {
                    _checkerOption.InfoPath = "/" + _checkerOption.InfoPath;
                }
                application.UseInfoCheck(_checkerOption.InfoPath);
                if (!_checkerOption.HealthPath.StartsWith("/"))
                {
                    _checkerOption.HealthPath = "/" + _checkerOption.HealthPath;
                }
                application.UseHealthCheck(_checkerOption.HealthPath);
            }

            //application.UseCheck();
        }

        /// <summary>
        /// Gets order of this startup configuration implementation
        /// </summary>
        public double Order { get; set; } = 0;
    }
}
