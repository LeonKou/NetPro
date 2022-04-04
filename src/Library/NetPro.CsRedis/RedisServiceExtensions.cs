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

using CSRedis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NetPro.CsRedis
{
    /// <summary>
    /// 
    /// </summary>
    public static class RedisServiceExtensions
    {
        /// <summary>
        /// 增加Cs.Redis服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddCsRedis<T>(this IServiceCollection services, IConfiguration configuration, Func<IServiceProvider, IList<ConnectionString>> connectionFactory = null) where T : class, ISerializer, new()
        {
            //切库参考：https://github.com/2881099/csredis/issues/63
            var option = configuration.GetSection(nameof(RedisCacheOption)).Get<RedisCacheOption>();

            if (option == null)
            {
                throw new ArgumentNullException(nameof(RedisCacheOption), $"未检测到NetPro.CsRedis配置节点{nameof(RedisCacheOption)}");
            }

            if (connectionFactory == null)
            {
                services.TryAddSingleton<ISerializer, T>();
                services.TryAddSingleton(option);
            }
            else
            {
                services.Replace(ServiceDescriptor.Singleton<ISerializer, T>());
                services.Replace(ServiceDescriptor.Singleton(sp =>
                {
                    var connection = connectionFactory.Invoke(sp);
                    var config = sp.GetRequiredService<IConfiguration>();
                    var option = config.GetSection(nameof(RedisCacheOption)).Get<RedisCacheOption>();
                    option!.ConnectionString = connection.ToList();
                    return option;
                }));
            }

            if (option.Enabled == false)
            {
                return services.AddNullCacheService(option);
            }

            return services.AddCsRedis();
        }

        /// <summary>
        /// 增加Cs.Redis服务
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        private static IServiceCollection AddCsRedis(this IServiceCollection services)
        {
            services.TryAddSingleton<IRedisManager, CsRedisManager>();
            services.TryAddSingleton((sp) =>
            {
                var option = sp.GetRequiredService<RedisCacheOption>();
                var idleBus = new IdleBus<CSRedisClient>(TimeSpan.FromSeconds(option.Idle == 0 ? 60 : option.Idle));
                foreach (var item in option.ConnectionString)
                {
                    idleBus.Register(item.Key, () =>
                    {
                        try
                        {
                            return new CSRedisClient(item.Value);
                        }
                        catch (Exception ex)
                        {
                            throw new ArgumentException($"请检查是否为非密码模式,Password必须为空字符串;请检查Database是否为0,只能在非集群模式下才可配置Database大于0；{ex}");
                        }
                    });
                }

                return idleBus;
            });

            return services;
        }

        /// <summary>
        /// 当禁用Redis时，用NullCache替换相关实现
        /// </summary>
        /// <param name="services"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        private static IServiceCollection AddNullCacheService(this IServiceCollection services, RedisCacheOption option)
        {
            Console.WriteLine($"Redis已关闭，当前驱动为NullCache!!!");
            services.TryAddSingleton<IRedisManager, NullCache>();
            var idleBusForNull = new IdleBus<CSRedisClient>(TimeSpan.FromSeconds(option.Idle == 0 ? 60 : option.Idle));
            services.TryAddSingleton(idleBusForNull);
            return services;
        }
    }
}
