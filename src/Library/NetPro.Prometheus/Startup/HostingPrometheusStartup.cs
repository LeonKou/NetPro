///*
// *  MIT License
// *  
// *  Copyright (c) 2021 LeonKou
// *  
// *  Permission is hereby granted, free of charge, to any person obtaining a copy
// *  of this software and associated documentation files (the "Software"), to deal
// *  in the Software without restriction, including without limitation the rights
// *  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// *  copies of the Software, and to permit persons to whom the Software is
// *  furnished to do so, subject to the following conditions:
// *  
// *  The above copyright notice and this permission notice shall be included in all
// *  copies or substantial portions of the Software.
// *  
// *  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// *  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// *  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// *  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// *  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// *  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// *  SOFTWARE.
// */

//using App.Metrics;
//using App.Metrics.AspNetCore;
//using App.Metrics.Formatters.Prometheus;
//using Microsoft.AspNetCore.Hosting;
//using Microsoft.Extensions.DependencyInjection;
//using NetPro.Prometheus;
//using System.Linq;

//[assembly: HostingStartup(typeof(Startup))]
//namespace NetPro.Prometheus
//{
//    internal sealed class Startup : IHostingStartup
//    {
//        public void Configure(IWebHostBuilder builder)
//        {
//            builder.ConfigureMetrics(AppMetrics.CreateDefaultBuilder()
//                    .OutputMetrics.AsPrometheusPlainText()
//                    .Build()).UseMetrics(options =>
//                    {
//                        options.EndpointOptions = endpointsOptions =>
//                        {
//                            endpointsOptions.MetricsTextEndpointOutputFormatter = Metrics.Instance.OutputMetricsFormatters.OfType<MetricsPrometheusTextOutputFormatter>().First();
//                        };
//                    });

//            // refer to https://www.app-metrics.io/reporting/collectors
//            builder.ConfigureServices(services =>
//            {
//                services.AddAppMetricsSystemMetricsCollector();
//                services.AddAppMetricsGcEventsMetricsCollector();
//            });
//        }
//    }
//}
