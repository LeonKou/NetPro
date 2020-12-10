using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetPro.Core.Configuration;
using NetPro.Core.Infrastructure;
using NetPro.Web.Core.Infrastructure.Extensions;

namespace Leon.XXX.Api
{
    /// <summary>
    /// ·ÖDomain²ã°æ±¾
    /// </summary>
    public class Startup
    {
        #region Fields

        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private IEngine _engine;
        private NetProOption _LeonConfig;

        #endregion

        #region Ctor

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="webHostEnvironment"></param>
        public Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }

        #endregion

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            (_engine, _LeonConfig) = services.ConfigureApplicationServices(_configuration, _webHostEnvironment);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        public void ConfigureContainer(ContainerBuilder builder)
        {
            _engine.RegisterDependencies(builder, _LeonConfig);
        }

        // 
        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.ConfigureRequestPipeline();
        }
    }
}
