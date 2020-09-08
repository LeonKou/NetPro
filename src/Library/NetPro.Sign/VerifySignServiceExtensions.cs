using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic.CompilerServices;
using NetPro.Web.Core.Filters;
using System;
using System.Collections.Generic;
using System.Text;

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
            var context = accessor?.HttpContext;
            return context?.RequestServices ?? ServiceProvider;
        }

        internal static T Resolve<T>()
        {
            return (T)GetServiceProvider().GetService(typeof(T));
        }
    }

    public static class VerifySignServiceExtensions
    {
        public static IServiceCollection AddVerifySign(this IServiceCollection services, Action<VerifySignOption> setupAction = null)
        {
            var configuration = services.BuildServiceProvider().GetRequiredService<IConfiguration>();

            var option = configuration.GetSection(nameof(VerifySignOption)).Get<VerifySignOption>();
            if (option.Enable)
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
                switch (option.Scheme.ToLower())
                {
                    case "global":
                        IoC.ServiceProvider = services.BuildServiceProvider();
                        services.AddControllers(config =>
                         {
                             config.Filters.Add(typeof(VerifySignFilter));//签名验证启动
                         });
                        break;
                    case "attribute":
                        IoC.ServiceProvider = services.BuildServiceProvider();
                        break;
                    default:
                        Console.WriteLine("签名以中间件方式启动");
                        break;
                }
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
        public string GetSignhHash(string message, string secret, string signMethod);
    }
}
