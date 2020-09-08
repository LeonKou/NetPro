using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetPro.Core.Infrastructure;
using NetPro.Web.Core.Infrastructure.Extensions;
using NetPro.TypeFinder;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;
using NetPro.Authentication;
using NetPro.ShareRequestBody;
using NetPro.Sign;

namespace NetPro.Web.Core.Infrastructure
{
    /// <summary>
    /// 签名
    /// </summary>
    public class SignStartup : INetProStartup
    {
        /// <summary>
        /// 添加 
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="configuration">Configuration root of the application</param>
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration, ITypeFinder typeFinder)
        {
            //签名
            if (configuration.GetValue<bool>("VerifySignOption:Enable", false))
            {
                services.AddVerifySign();
            }
        }

        /// <summary>
        /// 添加要使用的中间件
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public void Configure(IApplicationBuilder application)
        {
            if (application.ApplicationServices.GetRequiredService<IConfiguration>().GetValue<bool>("VerifySignOption:Enable", false))
            {
                application.UseGlobalSign();//签名
            }
        }

        /// <summary>
        /// Gets order of this startup configuration implementation
        /// </summary>
        public int Order
        {
            //authentication should be loaded before MVC
            get { return 110; }
        }
    }
}
