using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetPro.RedisManager;
using System;

namespace NetPro.Analysic
{
    /// <summary>
    /// 
    /// </summary>
    public static class RequestAnalysisExtensionService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddRequestAnalysic(this IServiceCollection services)
        {
            var configuration = services.BuildServiceProvider().GetRequiredService<IConfiguration>();
            try
            {
                services.AddRedisManager(configuration);
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("流量分析依赖NetPro.RedisManager 组件,请检查是否遗漏RedisCacheOption配置节点", ex);
            }
            services.AddHttpContextAccessor();
            var option = configuration.GetSection(nameof(RequestAnalysisOption)).Get<RequestAnalysisOption>();
            services.AddSingleton(option);
            return services;
        }

    }

    /// <summary>
    /// 
    /// </summary>
    public class RequestAnalysicBehaviorOptions
    {

        /// <summary>
        /// 
        /// </summary>
        public Func<bool, HttpContext> RequestAnalysicStateResponseFactory { get; set; }
    }
}
