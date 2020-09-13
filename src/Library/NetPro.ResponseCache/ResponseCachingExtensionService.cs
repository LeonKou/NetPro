using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetPro.ShareRequestBody;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetPro.ResponseCache
{
    public static class ResponseCachingExtensionService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddResponseCachingExtension(this IServiceCollection services)
        {
            services.AddResponseCaching(options =>
            {
                options.UseCaseSensitivePaths = false;
            });
            services.AddMemoryCache();
            services.AddShareRequestBody();//用户共享存放中间件读取的body

            var option = services.BuildServiceProvider().GetRequiredService<IConfiguration>()
                .GetSection(nameof(ResponseCacheOption)).Get<ResponseCacheOption>();
            services.AddSingleton(option);
            return services;
        }
    }

}
