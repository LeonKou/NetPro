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
using SNetPro.RedisManager;
using StackExchange.Redis;
using System;
using System.Collections.Generic;

namespace NetPro.RedisManager
{
    public static class RedisServiceExtensions
    {
        /// <summary>
        /// Redis服务，支持Csredis与StackExchangeRedis
        /// RedisCacheOption:RedisComponent=1>csredis;=2> StackExchangeRedis
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        /// <remarks>Csredis驱动下非集群模式Cluster必须为fasle，否则指定DataBase无效</remarks>
        //[Obsolete("废弃，请单独使用Csredis驱动或 StackExchange.Redis")]
        public static IServiceCollection AddRedisManager(this IServiceCollection services, IConfiguration configuration)
        {
            var option = configuration.GetSection(nameof(RedisCacheOption)).Get<RedisCacheOption>();
            if (option == null)
            {
                throw new ArgumentNullException(nameof(RedisCacheOption), $"未检测到NetPro.RedisManager配置节点{nameof(RedisCacheOption)}");
            }
            services.AddRedisManager(option);
            return services;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="redisCacheOption"></param>
        /// <returns></returns>
        //[Obsolete("废弃，请单独使用Csredis驱动或 StackExchange.Redis")]
        public static IServiceCollection AddRedisManager(this IServiceCollection services, RedisCacheOption redisCacheOption)
        {
            var redisCacheComponent = redisCacheOption?.RedisComponent ?? RedisCacheComponentEnum.NullRedis;
            services.AddMemoryCache();
            switch (redisCacheComponent)
            {
                case RedisCacheComponentEnum.CSRedisCore:
                    services.AddCsRedis(redisCacheOption);
                    break;
                case RedisCacheComponentEnum.StackExchangeRedis:
                    services.AddStackExchangeRedis(redisCacheOption);
                    break;
                case RedisCacheComponentEnum.NullRedis:
                    services.AddSingleton<IRedisManager, NullCache>();
                    break;
                default:
                    services.AddSingleton(redisCacheOption);
                    services.AddSingleton<IRedisManager, NullCache>();
                    break;
            }
            return services;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="option"></param>
        /// <returns></returns>

        //[Obsolete("废弃，请单独使用Csredis驱动或 StackExchange.Redis")]
        public static IServiceCollection AddCsRedis(this IServiceCollection services, RedisCacheOption option)
        {
            services.AddSingleton<ISerializer, NewtonsoftSerializer>();
            services.AddSingleton(option);
            List<string> csredisConns = new List<string>();
            string password = option.Password;
            int defaultDb = option.Database;
            string ssl = option.SslHost;
            string keyPrefix = option.DefaultCustomKey;
            int writeBuffer = 10240;
            int poolsize = option.PoolSize == 0 ? 10 : option.PoolSize;
            int timeout = option.ConnectionTimeout;
            foreach (var e in option.Endpoints)
            {
                string server = e.Host;
                int port = e.Port;
                if (string.IsNullOrWhiteSpace(server) || port <= 0) { continue; }
                csredisConns.Add($"{server}:{port},password={password},defaultDatabase={defaultDb},poolsize={poolsize},ssl={ssl},writeBuffer={writeBuffer},prefix={keyPrefix},preheat={option.Preheat},idleTimeout={timeout},testcluster={option.Cluster}");
            }

            CSRedis.CSRedisClient csredis;

            try
            {
                csredis = new CSRedis.CSRedisClient(null, csredisConns.ToArray());
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"请检查是否为非密码模式,Password必须为空字符串;请检查Database是否为0,只能在非集群模式下才可配置Database大于0；{ex}");
            }

            //RedisHelper.Initialization(csredis);
            services.AddSingleton(csredis);
            services.AddSingleton<IRedisManager, CsRedisManager>();

            /*注入stackexchange驱动，防止CSRedis驱动下获取IDatabase异常 */
            #region   注入stackexchange驱动
            var configurationOptions = new ConfigurationOptions
            {
                KeepAlive = 180,
                ConnectTimeout = option.ConnectionTimeout,
                Password = option.Password,
                Ssl = option.IsSsl,
                SslHost = option.SslHost,
                AbortOnConnectFail = false,
                AsyncTimeout = option.ConnectionTimeout,
                DefaultDatabase = option.Database,
            };

            foreach (var endpoint in option.Endpoints)
            {
                configurationOptions.EndPoints.Add(endpoint.Host, endpoint.Port);
            }

            var connect = ConnectionMultiplexer.Connect(configurationOptions);
            //注册如下事件
            connect.ConnectionFailed += MuxerConnectionFailed;
            connect.ConnectionRestored += MuxerConnectionRestored;
            connect.ErrorMessage += MuxerErrorMessage;
            connect.ConfigurationChanged += MuxerConfigurationChanged;
            connect.HashSlotMoved += MuxerHashSlotMoved;
            connect.InternalError += MuxerInternalError;
            services.AddSingleton(connect);
            //services.Add(ServiceDescriptor.Singleton(connect));
            #endregion

            return services;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        //[Obsolete("废弃，请单独使用Csredis驱动或 StackExchange.Redis")]
        public static IServiceCollection AddCsRedis(this IServiceCollection services, IConfiguration config)
        {
            var option = new RedisCacheOption(config);
            services.AddCsRedis(option);
            return services;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        //[Obsolete("废弃，请单独使用Csredis驱动或 StackExchange.Redis")]
        public static IServiceCollection AddStackExchangeRedis(this IServiceCollection services, IConfiguration config)
        {
            var redisOption = new RedisCacheOption(config);

            services.AddStackExchangeRedis(redisOption);
            return services;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="redisOption"></param>
        /// <returns></returns>
        //[Obsolete("废弃，请单独使用Csredis驱动或 StackExchange.Redis")]
        public static IServiceCollection AddStackExchangeRedis(this IServiceCollection services, RedisCacheOption redisOption)
        {
            services.AddSingleton<ISerializer, NewtonsoftSerializer>();
            services.AddSingleton(redisOption);
            var configurationOptions = new ConfigurationOptions
            {
                KeepAlive = 180,
                ConnectTimeout = redisOption.ConnectionTimeout,
                Password = redisOption.Password,
                Ssl = redisOption.IsSsl,
                SslHost = redisOption.SslHost,
                AbortOnConnectFail = false,
                AsyncTimeout = redisOption.ConnectionTimeout,
                DefaultDatabase = redisOption.Database,
            };

            foreach (var endpoint in redisOption.Endpoints)
            {
                configurationOptions.EndPoints.Add(endpoint.Host, endpoint.Port);
            }

            var connect = ConnectionMultiplexer.Connect(configurationOptions);
            //注册如下事件
            connect.ConnectionFailed += MuxerConnectionFailed;
            connect.ConnectionRestored += MuxerConnectionRestored;
            connect.ErrorMessage += MuxerErrorMessage;
            connect.ConfigurationChanged += MuxerConfigurationChanged;
            connect.HashSlotMoved += MuxerHashSlotMoved;
            connect.InternalError += MuxerInternalError;

            //services.Add(ServiceDescriptor.Singleton(connect));
            services.AddSingleton(connect);
            services.AddSingleton<IRedisManager, StackExchangeRedisManager>();

            /*注入CSRedis驱动，防止Stackexchange驱动下获取执行RedisHelper异常*/
            #region  注入CSRedis驱动
            services.AddSingleton(redisOption);
            List<string> csredisConns = new List<string>();
            string password = redisOption.Password;
            int defaultDb = redisOption.Database;
            string ssl = redisOption.SslHost;
            string keyPrefix = redisOption.DefaultCustomKey;
            int writeBuffer = 10240;
            int poolsize = redisOption.PoolSize == 0 ? 10 : redisOption.PoolSize;
            int timeout = redisOption.ConnectionTimeout;
            foreach (var e in redisOption.Endpoints)
            {
                string server = e.Host;
                int port = e.Port;
                if (string.IsNullOrWhiteSpace(server) || port <= 0) { continue; }
                csredisConns.Add($"{server}:{port},password={password},defaultDatabase={defaultDb},poolsize={poolsize},ssl={ssl},writeBuffer={writeBuffer},prefix={keyPrefix},preheat={redisOption.Preheat},idleTimeout={timeout},testcluster={redisOption.Cluster}");
            }

            CSRedis.CSRedisClient csredis;

            try
            {
                csredis = new CSRedis.CSRedisClient(null, csredisConns.ToArray());
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Check the configuration for redis;{ex}");
            }

            //RedisHelper.Initialization(csredis);
            services.AddSingleton(csredis);

            #endregion

            return services;
        }

        #region 事件

        /// <summary>
        /// 配置更改时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerConfigurationChanged(object sender, EndPointEventArgs e)
        {
            Console.WriteLine("Configuration changed: " + e.EndPoint);
        }

        /// <summary>
        /// 发生错误时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerErrorMessage(object sender, RedisErrorEventArgs e)
        {
            Console.WriteLine("ErrorMessage: " + e.Message);
        }

        /// <summary>
        /// 重新建立连接之前的错误
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerConnectionRestored(object sender, ConnectionFailedEventArgs e)
        {
            Console.WriteLine("ConnectionRestored: " + e.EndPoint);
        }

        /// <summary>
        /// 连接失败 ， 如果重新连接成功你将不会收到这个通知
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerConnectionFailed(object sender, ConnectionFailedEventArgs e)
        {
            Console.WriteLine("Retry：Endpoint failed: " + e.EndPoint + ", " + e.FailureType + (e.Exception == null ? "" : (", " + e.Exception.Message)));
        }

        /// <summary>
        /// 更改集群
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerHashSlotMoved(object sender, HashSlotMovedEventArgs e)
        {
            Console.WriteLine("HashSlotMoved:NewEndPoint" + e.NewEndPoint + ", OldEndPoint" + e.OldEndPoint);
        }

        /// <summary>
        /// redis类库错误
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerInternalError(object sender, InternalErrorEventArgs e)
        {
            Console.WriteLine("InternalError:Message" + e.Exception.Message);
        }

        #endregion 事件
    }
}
