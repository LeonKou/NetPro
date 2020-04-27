using Microsoft.ApplicationInsights.AspNetCore;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.SnapshotCollector;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace NetPro.Web.Core.Infrastructure.ApplicationInsights
{
    /// <summary>
    /// 
    /// </summary>
    internal class SnapshotCollectorTelemetryProcessorFactory : ITelemetryProcessorFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public SnapshotCollectorTelemetryProcessorFactory(IServiceProvider serviceProvider) =>
            _serviceProvider = serviceProvider;

        public ITelemetryProcessor Create(ITelemetryProcessor next)
        {
            var snapshotConfigurationOptions = _serviceProvider.GetService<IOptions<SnapshotCollectorConfiguration>>();
            return new SnapshotCollectorTelemetryProcessor(next, configuration: snapshotConfigurationOptions.Value);
        }
    }
}
