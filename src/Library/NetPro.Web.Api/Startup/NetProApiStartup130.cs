using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetPro.Core.Infrastructure;
using NetPro.Swagger;
using NetPro.TypeFinder;
using NetPro.Web.Api.Filters;

namespace NetPro.Web.Api
{
    /// <summary>
    /// 配置应用程序启动时MVC需要的中间件
    /// </summary>
    public class NetProApiStartup : INetProStartup
    {
        /// <summary>
        /// Add and configure any of the middleware
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="configuration">Configuration root of the application</param>
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration, ITypeFinder typeFinder)
        {
            //配置 mvc服务
            var mvcBuilder = services.AddControllers(config =>
            {
                config.Filters.Add(typeof(CustomAuthorizeFilter));//用户权限验证过滤器                                                                    
                                                                  //同类型的过滤按添加先后顺序执行,第一个最先执行；ApiController特性下，
                                                                  //过滤器无法挡住过滤验证失败，故无法统一处理，只能通过ConfigureApiBehaviorOptions              
            });
            services.AddNetProSwagger(configuration);
        }

        public void Configure(IApplicationBuilder application)
        {
            //TODO流量分析等其他中间件            
            application.UseRouting();
            application.UseCors("CorsPolicy");
            application.UseAuthorization();
            application.UseNetProSwagger();
            application.UseEndpoints(s =>
            {
                s.MapControllers();
            });
        }

        /// <summary>
        /// Gets order of this startup configuration implementation
        /// </summary>
        public int Order
        {
            //MVC should be loaded last
            get { return 130; }
        }
    }
}
