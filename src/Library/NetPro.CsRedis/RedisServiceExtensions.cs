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
using Microsoft.Extensions.Logging;
using System;

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
        public static IServiceCollection AddCsRedis<T>(this IServiceCollection services, IConfiguration configuration) where T : class, ISerializer, new()
        {
            //切库参考：https://github.com/2881099/csredis/issues/63
            var option = configuration.GetSection(nameof(RedisCacheOption)).Get<RedisCacheOption>();

            if (option == null)
            {
                throw new ArgumentNullException(nameof(RedisCacheOption), $"未检测到NetPro.CsRedis配置节点{nameof(RedisCacheOption)}");
            }

            return services.AddCsRedis<T>(sp => option);
        }

        /// <summary>
        /// 增加Cs.Redis服务
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="RedisCacheOption">The redis configration.</param>
        /// <typeparam name="T">The typof of serializer. <see cref="ISerializer" />.</typeparam>
        public static IServiceCollection AddCsRedis<T>(
            this IServiceCollection services,
             Func<IServiceProvider, RedisCacheOption> RedisCacheOption)
            where T : class, ISerializer, new()
        {
            services.AddSingleton<ISerializer, T>();
            return services.AddCsRedis(RedisCacheOption);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="redisCacheOption"></param>
        /// <returns></returns>
        public static IServiceCollection AddCsRedis(this IServiceCollection services, Func<IServiceProvider, RedisCacheOption> redisCacheOption)
        {
            // redis连接串示例 var connectionString = "127.0.0.1:6379,password=123,defaultDatabase=0,poolsize=10,preheat=20,ssl=false,writeBuffer=10240,prefix=key前辍,testcluster=false,idleTimeout=10";
            services.AddSingleton(redisCacheOption);
            var option = redisCacheOption.Invoke(services.BuildServiceProvider());
            services.AddSingleton(option);

            if (option?.Enabled ?? false)
            {
                services.AddSingleton<IRedisManager, CsRedisManager>();
            }
            else
            {
                //禁用，用NullCache实例化，防止现有注入失败
                var _logger = services.BuildServiceProvider().GetRequiredService<ILogger<CsRedisManager>>();
                _logger.LogInformation($"Redis已关闭，当前驱动为NullCache!!!");
                services.AddSingleton<IRedisManager, NullCache>();
                return services;
            }

            //CSRedisClient csredis;
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
            services.AddSingleton(idleBus);

            //静态RedisHelper方式初始化
            //RedisHelper.Initialization(csredis);

            return services;
        }
    }
}
