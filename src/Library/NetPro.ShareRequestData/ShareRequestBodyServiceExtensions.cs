using Microsoft.Extensions.DependencyInjection;

namespace NetPro.ShareRequestBody
{
    public static class ShareRequestBodyServiceExtensions
    {
        /// <summary>
        /// 共享请求或者响应中的Body
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddShareRequestBody(this IServiceCollection services)
        {
            //request的body通过中间件获取
            //Response的body通过过滤器获取
            services.AddScoped(s => new ResponseCacheData { ContentType = "application/json" });//用于各中间件共享响应body
            services.AddScoped(s => new RequestCacheData());//用于各中间件共享请求body

            return services;
        }
    }
}
