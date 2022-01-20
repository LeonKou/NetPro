/*
 *  MIT License
 *  
 *  Copyright (c) 2021 LeonKou
 *  
 *  Permission is hereby granted, free of charge, to any person obtaining a copy
 *  of this software and associated documentation files (the "Software"), to deal
 *  in the Software without restriction, including without limitation the rights
 *  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 *  copies of the Software, and to permit persons to whom the Software is
 *  furnished to do so, subject to the following conditions:
 *  
 *  The above copyright notice and this permission notice shall be included in all
 *  copies or substantial portions of the Software.
 *  
 *  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 *  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 *  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 *  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 *  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 *  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 *  SOFTWARE.
 */

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetPro.ShareRequestBody;

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
            if (option != null)
                services.AddSingleton(option);
            return services;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddResponseCachingExtension(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddResponseCaching(options =>
            {
                options.UseCaseSensitivePaths = false;
            });
            services.AddMemoryCache();
            services.AddShareRequestBody();//用户共享存放中间件读取的body

            var option = configuration.GetSection(nameof(ResponseCacheOption)).Get<ResponseCacheOption>();
            if (option != null)
                services.AddSingleton(option);
            return services;
        }
    }

}
