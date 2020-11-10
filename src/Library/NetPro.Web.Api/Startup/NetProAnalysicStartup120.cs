using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetPro.Analysic;
using NetPro.Core.Infrastructure;
using NetPro.Swagger;
using NetPro.TypeFinder;
using NetPro.Web.Api.Filters;

namespace NetPro.Web.Api.Startup
{
    public class NetProAnalysicStartup120 : INetProStartup
    {
        /// <summary>
        /// Add and configure any of the middleware
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="configuration">Configuration root of the application</param>
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration, ITypeFinder typeFinder)
        {
#pragma warning disable CS0612 // Type or member is obsolete
            services.AddRequestAnalysic();
#pragma warning restore CS0612 // Type or member is obsolete
        }

        public void Configure(IApplicationBuilder application)
        {
            application.UseRequestAnalysis();
        }

        /// <summary>
        /// Gets order of this startup configuration implementation
        /// </summary>
        public int Order
        {
            get { return 120; }
        }
    }
}
