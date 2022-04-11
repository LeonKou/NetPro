using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.NetPro;

namespace NetPro.AutoMapper
{
    internal sealed class AutoMapperStartup : INetProStartup, System.NetPro.Startup.__._
    {
        /// <summary>
        /// AddAutoMapper
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="configuration">Configuration root of the application</param>
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration, ITypeFinder typeFinder)
        {
            AutoMapperExtensionService.AddNetProAutoMapper(services, typeFinder);
        }

        public void Configure(IApplicationBuilder application, IWebHostEnvironment env)
        {
        }

        /// <summary>
        /// Gets order of this startup configuration implementation
        /// </summary>
        public double Order { get; set; } = 0;
    }
}
