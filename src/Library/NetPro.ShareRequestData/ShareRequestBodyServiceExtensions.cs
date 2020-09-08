using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

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
            services.AddScoped(s => new ResponseCacheData());//用于各中间件共享响应body
            services.AddScoped(s => new RequestCacheData());//用于各中间件共享请求body

            return services;
        }
    }
}
