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

using App.Metrics;
using App.Metrics.AspNetCore.Endpoints;
using App.Metrics.Extensions.Configuration;
using App.Metrics.Formatters.Prometheus;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetPro.Prometheus;
using System.Linq;
using System.NetPro;

[assembly: HostingStartup(typeof(Startup))]
namespace NetPro.Prometheus
{
    internal sealed class Startup //: IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            // refer to https://www.app-metrics.io/reporting/collectors
            builder.ConfigureServices(services =>
            {
                var configuration = services.BuildServiceProvider().GetService<IConfiguration>();
                services.Configure<MetricsEndpointsHostingOptions>(configuration.GetSection(nameof(MetricsEndpointsHostingOptions)));

                var metrics = AppMetrics.CreateDefaultBuilder().Configuration.ReadFrom(configuration).OutputMetrics.AsPrometheusPlainText().Build();
                services.AddMetrics(metrics)
                        .AddMetricsTrackingMiddleware(configuration)
                        .AddMetricsEndpoints(configuration, option =>
                        {
                            option.MetricsTextEndpointOutputFormatter = Metrics.Instance.OutputMetricsFormatters.OfType<MetricsPrometheusTextOutputFormatter>().First();
                        });

                services.AddAppMetricsSystemMetricsCollector();
                services.AddAppMetricsGcEventsMetricsCollector();
            });

            // because Configure can only execute once, this method is not used
            // refer to https://stackoverflow.com/a/65061961/14700529
            builder.Configure(application =>
            {
                application.UseMetricsAllMiddleware();
                application.UseMetricsAllEndpoints();
            });
        }
    }
}
