using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.NetPro;

namespace NetPro.Sign
{
    /// <summary>
    /// 支持签名
    /// appsetting.json VerifySignOption:Enabled=true 打开签名
    /// </summary>
    internal sealed class SignStartup : INetProStartup, System.NetPro.Startup.__._
    {
        /// <summary>
        /// 添加 
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="configuration">Configuration root of the application</param>
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration, ITypeFinder typeFinder)
        {
            //签名
            services.AddVerifySign();
            //if (configuration.GetValue<bool>("VerifySignOption:Disabled", false))
            //{
            //    services.AddVerifySign();
            //}
        }

        /// <summary>
        /// 添加要使用的中间件
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public void Configure(IApplicationBuilder application, IWebHostEnvironment env)
        {
            if (application.ApplicationServices.GetRequiredService<IConfiguration>().GetValue<bool>("VerifySignOption:Enabled", false))
            {
                application.UseGlobalSign();//签名
            }
        }

        /// <summary>
        /// Gets order of this startup configuration implementation
        /// </summary>
        public double Order { get; set; } = 600; //authentication should be loaded before MVC
    }
}
