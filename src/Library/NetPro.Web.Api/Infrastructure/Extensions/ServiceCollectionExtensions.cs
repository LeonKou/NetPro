using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NetPro.Web.Api.Filters;
using NetPro.Web.Core.Models;
using System.Text;

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
            var mvcBuilder = services.AddControllers(config =>
            {
                config.Filters.Add(typeof(CustomAuthorizeFilter));//用户权限验证过滤器
                                                                  //config.Filters.Add(new CustomAuthorizeFilter(new AuthorizationPolicyBuilder()
                                                                  //    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                                                                  //    .RequireAuthenticatedUser().Build())); //controller统一认证 重写401 403返回值

                //同类型的过滤按添加先后顺序执行,第一个最先执行；ApiController特性下，过滤器无法挡住过滤验证失败，故无法统一处理，只能通过ConfigureApiBehaviorOptions
                //config.Filters.Add(new ViewModelStateActionFilter());//viewmodel数据合法性验证过滤器

            })
                .ConfigureApiBehaviorOptions(options =>
                {                              
                    options.InvalidModelStateResponseFactory = context =>
                    {
                        StringBuilder stringBuilder = new StringBuilder();
                        foreach (var keyModelStatePair in context.ModelState)
                        {
                            var key = keyModelStatePair.Key;
                            var errors = keyModelStatePair.Value.Errors;
                            if (errors != null && errors.Count > 0)
                            {
                                stringBuilder.Append(errors[0].ErrorMessage);
                            }
                        }
                        return new BadRequestObjectResult(new ApiResultModel { ErrorCode = -1, Msg = $"数据验证失败--详情：{stringBuilder}" })
                        {
                            ContentTypes = { "application/problem+json", "application/problem+xml" }
                        };
                    };
                })
            ;
            return mvcBuilder;
        }
    }
}