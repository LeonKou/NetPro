using App.Metrics;
using App.Metrics.AspNetCore.Endpoints;
using App.Metrics.Extensions.Configuration;
using App.Metrics.Formatters.Prometheus;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.NetPro;

namespace NetPro.Prometheus
{
    internal sealed class PrometheusStartup : INetProStartup, System.NetPro.Startup.__._
    {
        public double Order { get; set; } = 0;

        public void Configure(IApplicationBuilder application, IWebHostEnvironment env)
        {
            application.UseMetricsAllMiddleware();
            application.UseMetricsAllEndpoints();
        }

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration = null, ITypeFinder typeFinder = null)
        {
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
        }
    }

}
