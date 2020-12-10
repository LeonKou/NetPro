using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System;

namespace NetPro.Sign
{
    /// <summary>
    /// 
    /// </summary>
    internal static class IoC
    {
        internal static IServiceProvider ServiceProvider { get; set; }

        private static IServiceProvider GetServiceProvider()
        {
            if (ServiceProvider == null)
                return null;
            var accessor = ServiceProvider?.GetService<IHttpContextAccessor>();
            return accessor?.HttpContext?.RequestServices ?? ServiceProvider;
        }

        internal static T Resolve<T>()
        {
            return (T)GetServiceProvider().GetService(typeof(T));
        }
    }

    public static class VerifySignServiceExtensions
    {
        /// <summary>
        /// 接口签名提供特新方式与中间件方式，建议只使用一种方式
        /// </summary>
        /// <param name="services"></param>
        /// <param name="setupAction"></param>
        /// <returns></returns>
        public static IServiceCollection AddVerifySign(this IServiceCollection services, Action<VerifySignOption> setupAction = null)
        {
            var configuration = services.BuildServiceProvider().GetService<IConfiguration>();

            var option = configuration.GetSection(nameof(VerifySignOption)).Get<VerifySignOption>();
            if (option?.Enabled??false)
            {
                if (setupAction != null) services.ConfigureSign(setupAction);

                var tempOption = services.BuildServiceProvider().GetService<IOptions<VerifySignOption>>().Value;
                option.OperationFilterDescriptors.AddRange(tempOption.OperationFilterDescriptors);
                services.TryAddSingleton(option);

                services.AddScoped<IOperationFilter, VerifySignDefaultFilter>();//注入默认处理

                foreach (var item in tempOption.OperationFilterDescriptors)
                {
                    services.AddScoped(typeof(IOperationFilter), item);//覆盖默认处理
                }
                IoC.ServiceProvider = services.BuildServiceProvider();

            }
            else
            {
                IoC.ServiceProvider = services.BuildServiceProvider();  //for VerifySignAttribute
                Console.WriteLine("签名验证已关闭");
            }

            return services;
        }

        private static void ConfigureSign(
           this IServiceCollection services,
           Action<VerifySignOption> setupAction)
        {
            services.Configure(setupAction);
        }
    }

    /// <summary>
    /// 签名摘要自定义实现
    /// TODO 根据接口单一原则实现两个接口
    /// </summary>
    public interface IOperationFilter
    {
        /// <summary>
        /// 获取AK/SK的Secret
        /// </summary>
        /// <param name="appid"></param>
        /// <returns></returns>
        public string GetSignSecret(string appid);

        /// <summary>
        /// 定义摘要算法
        /// </summary>
        /// <param name="message">待摘要的内容</param>
        /// <param name="secret">Ak/SK的secret</param>
        /// <param name="signMethod">客户端要求的加密方式;hmac，md5，hmac-sha256</param>
        /// <returns></returns>
        public string GetSignhHash(string message, string secret, EncryptEnum signMethod = EncryptEnum.Default);
    }
}
