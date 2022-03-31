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
        /// <typeparam name="GetConnections">获取连接传集合类
        /// 继承IConnectionsFactory</typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static ICSRedisBuilder AddCsRedis<GetConnections>(this IServiceCollection services) where GetConnections : class, IConnectionsFactory
        {
            services.AddTransient<IConnectionsFactory, GetConnections>();
            return new CSRedisBuilder(services);
        }

        /// <summary>
        /// 增加Cs.Redis服务
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static ICSRedisBuilder AddCsRedis(this IServiceCollection services)
        {
            //如已经注册自定义服务，跳过
            var serviceProviderIsService = services.BuildServiceProvider().GetRequiredService<IServiceProviderIsService>();
            if (!serviceProviderIsService.IsService(typeof(IConnectionsFactory)))
            {
                services.AddTransient<IConnectionsFactory, DefaultConnections>();
            }

            return new CSRedisBuilder(services);
        }

        /// <summary>
        /// 增加Cs.Redis服务
        /// </summary>
        /// <typeparam name="T">The typof of serializer. <see cref="ISerializer" />.</typeparam>
        public static IServiceCollection Build<T>(
            this ICSRedisBuilder builder,
            IConfiguration configuration)
            where T : class, ISerializer, new()
        {
            builder.Services.AddSingleton<ISerializer, T>();
            var option = new RedisCacheOption(configuration);
            builder.Services.AddSingleton(option);
            return builder.AddCsRedisConnecter(option);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="redisCacheOption"></param>
        /// <returns></returns>
        private static IServiceCollection AddCsRedisConnecter(this ICSRedisBuilder builder, RedisCacheOption redisCacheOption)
        {
            // redis连接串示例 var connectionString = "127.0.0.1:6379,password=123,defaultDatabase=0,poolsize=10,preheat=20,ssl=false,writeBuffer=10240,prefix=key前辍,testcluster=false,idleTimeout=10";
            //builder.Services.AddSingleton(redisCacheOption);
            //var option = redisCacheOption;// redisCacheOption.Invoke(builder.Services.BuildServiceProvider());
            //var serviceProviderIsService = builder.Services.BuildServiceProvider().GetRequiredService<IServiceProviderIsService>();
            var connectionsFactory = builder.Services.BuildServiceProvider().GetRequiredService<IConnectionsFactory>();
            if (connectionsFactory is not DefaultConnections)
            {
                redisCacheOption.ConnectionString = connectionsFactory.GetConnectionStrings().ToList();
                builder.Services.AddSingleton(redisCacheOption);//用最新连接串覆盖
            }

            if (redisCacheOption?.Enabled ?? false)
            {
                builder.Services.AddSingleton<IRedisManager, CsRedisManager>();
            }
            else
            {
                //禁用，用NullCache实例化，防止现有注入失败
                var _logger = builder.Services.BuildServiceProvider().GetRequiredService<ILogger<CsRedisManager>>();
                _logger.LogInformation($"Redis已关闭，当前驱动为NullCache!!!");
                builder.Services.AddSingleton<IRedisManager, NullCache>();

                var idleBusForNull = new IdleBus<CSRedisClient>(TimeSpan.FromSeconds(redisCacheOption.Idle == 0 ? 60 : redisCacheOption.Idle));
                builder.Services.AddSingleton(idleBusForNull);
                return builder.Services;
            }

            //CSRedisClient csredis;
            var idleBus = new IdleBus<CSRedisClient>(TimeSpan.FromSeconds(redisCacheOption.Idle == 0 ? 60 : redisCacheOption.Idle));

            foreach (var item in redisCacheOption.ConnectionString)
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
            builder.Services.AddSingleton(idleBus);

            //静态RedisHelper方式初始化
            //RedisHelper.Initialization(csredis);
            return builder.Services;
        }
    }
}
