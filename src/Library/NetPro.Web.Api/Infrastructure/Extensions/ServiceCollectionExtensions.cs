using Microsoft.Extensions.DependencyInjection;  
using NetPro.Web.Api.Filters;

namespace NetPro.Web.Api.Infrastructure.Extensions
{
    /// <summary>
    /// Represents extensions of IServiceCollection
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add and configure MVC for the application
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <returns>A builder for configuring MVC services</returns>
        public static IMvcBuilder AddNetProApi(this IServiceCollection services)
        {
            //var NetProOption = services.GetNetProConfig();

            //add basic MVC feature    AddMvc
               
            var mvcBuilder = services.AddControllers(config =>
            {
                config.Filters.Add(typeof(CustomAuthorizeFilter));//用户权限验证过滤器
                //config.Filters.Add(new CustomAuthorizeFilter(new AuthorizationPolicyBuilder()
                //    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                //    .RequireAuthenticatedUser().Build())); //controller统一认证 重写401 403返回值

                //同类型的过滤按添加先后顺序执行,第一个最先执行
                config.Filters.Add(new ViewModelStateActionFilter());//viewmodel数据合法性验证过滤器
                
            });
            return mvcBuilder;
        }
    }
}