using StackExchange.Redis.Extensions.AspNetCore.Midllewares;
using System;

namespace Microsoft.AspNetCore.Builder
{
    /// <summary>
    /// Redis的一组扩展方法。
    /// </summary>
    public static class IApplicationBuilderExtensions
    {
        /// <summary>
        ///  启用Redis配置中间件
        /// </summary>
        /// <param name="application"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IApplicationBuilder UserRedisInformation(this IApplicationBuilder application, Action<RedisMiddlewareAccessOptions> options = null)
        {
            var opt = new RedisMiddlewareAccessOptions();
            options?.Invoke(opt);

            application.UseMiddleware<RedisInformationMiddleware>(opt);
            return application;
        }
    }
}
