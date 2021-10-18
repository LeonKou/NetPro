using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NetPro.Core.Configuration;
using NetPro.Core.Infrastructure;
using NetPro.TypeFinder;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;

namespace NetPro.ResponseCache
{
    public class GlobalizationStartup500 : INetProStartup
    {
        /// <summary>
        /// Add and configure any of the middleware
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="configuration">Configuration root of the application</param>
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration, ITypeFinder typeFinder)
        {
            services.AddResponseCachingExtension();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseGetResponseCaching();
        }

        /// <summary>
        /// Gets order of this startup configuration implementation
        /// </summary>
        public double Order
        {
            get { return 500; }
        }
    }
}
