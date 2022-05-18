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

using FreeRedis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NetPro.FreeRedis
{
    /// <summary>
    /// 
    /// </summary>
    public static class RedisServiceExtensions
    {
        /// <summary>
        /// 增加FreeRedis服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddFreeRedis(this IServiceCollection services, IConfiguration configuration, Func<IServiceProvider, IList<ConnectionString>> connectionFactory = null)
        {
            var option = configuration.GetSection(nameof(RedisCacheOption)).Get<RedisCacheOption>();

            if (option == null)
            {
                throw new ArgumentNullException(nameof(RedisCacheOption), $"未检测到NetPro.FreeRedis配置节点{nameof(RedisCacheOption)}");
            }

            if (connectionFactory == null)
            {
                services.TryAddSingleton(option);
            }
            else
            {
                services.Replace(ServiceDescriptor.Singleton(sp =>
                {
                    var connection = connectionFactory.Invoke(sp);
                    var config = sp.GetRequiredService<IConfiguration>();
                    var option = config.GetSection(nameof(RedisCacheOption)).Get<RedisCacheOption>();
                    option!.ConnectionString = connection.ToList();
                    return option;
                }));
            }

            return services.AddFreeRedis();
        }

        /// <summary>
        /// 增加FreeRedis服务
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        private static IServiceCollection AddFreeRedis(this IServiceCollection services)
        {
            services.TryAddSingleton((sp) =>
            {
                var option = sp.GetRequiredService<RedisCacheOption>();
                var idleBus = new IdleBus<RedisClient>(TimeSpan.FromSeconds(option.Idle == 0 ? 60 : option.Idle));
                foreach (var item in option.ConnectionString)
                {
                    idleBus.Register(item.Key, () =>
                    {
                        try
                        {
                            var redisClient = new RedisClient(item.Value);
                            redisClient.Serialize = obj => JsonConvert.SerializeObject(obj);
                            redisClient.Deserialize = (json, type) => JsonConvert.DeserializeObject(json, type);
                            return redisClient;
                        }
                        catch (Exception ex)
                        {
                            throw new ArgumentException(ex.Message);
                        }
                    });
                }

                return idleBus;
            });

            return services;
        }
    }
}
