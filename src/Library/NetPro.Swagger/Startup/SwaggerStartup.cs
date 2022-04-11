using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.NetPro;

namespace NetPro.Swagger
{
    /// <summary>
    /// Swagger接口文档工具 
    /// </summary>
    internal sealed class SwaggerStartup : INetProStartup, System.NetPro.Startup.__._
    {
        /// <summary>
        /// /
        /// </summary>
        public double Order { get; set; } = 100;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="application"></param>
        public void Configure(IApplicationBuilder application, IWebHostEnvironment env)
        {
            application.UseNetProSwagger();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <param name="typeFinder"></param>
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration = null, ITypeFinder typeFinder = null)
        {
            services.AddNetProSwagger(configuration, typeFinder);
        }
    }
}
