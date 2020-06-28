﻿using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetPro.Sign
{
    public static class VerifySignExtensions
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

                services.AddScoped<IOperationFilter, VerifySignCommon>();//注入默认处理

                foreach (var item in tempOption.OperationFilterDescriptors)
                {
                    services.AddScoped(typeof(IOperationFilter), item);//覆盖默认处理
                }
                services.AddControllers(config =>
                {
                    config.Filters.Add(typeof(VerifySignFilter));//签名验证启动
                });
            }
            else
                Console.WriteLine("签名验证已关闭");

            return services;
        }

        public static void ConfigureSign(
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
        /// <returns></returns>
        public string GetSignhHash(string message, string secret);
    }
}
