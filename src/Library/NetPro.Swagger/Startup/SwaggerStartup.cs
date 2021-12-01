using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetPro.TypeFinder;
using NetPro.Swagger;
using Microsoft.AspNetCore.Hosting;

namespace NetPro.Swagger
{
    /// <summary>
    /// Swagger接口文档工具 
    /// </summary>
    public class SwaggerStartup : INetProStartup
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
            services.AddNetProSwagger(configuration);
        }
    }
}
