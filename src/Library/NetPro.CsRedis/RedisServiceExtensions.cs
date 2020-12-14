using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NetPro.CsRedis;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace NetPro.CsRedis
{
    public static class RedisServiceExtensions
    {
        /// <summary>
        /// 增加StackExchange.Redis服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddCsRedis<T>(this IServiceCollection services, IConfiguration configuration) where T : class, ISerializer, new()
        {
            var option = configuration.GetSection(nameof(RedisCacheOption)).Get<RedisCacheOption>();

            if (option == null)
            {
                throw new ArgumentNullException(nameof(RedisCacheOption),$"未检测到NetPro.CsRedis配置节点{nameof(RedisCacheOption)}");
            }

            return services.AddCsRedis<T>(sp => option);
        }

        /// <summary>
        /// 增加StackExchange.Redis服务
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
            services.AddSingleton(redisCacheOption);
            var option = redisCacheOption.Invoke(services.BuildServiceProvider());
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

            RedisHelper.Initialization(csredis);
            if (option?.Enabled ?? false)
                services.AddSingleton<IRedisManager, CsRedisManager>();
            else
            {
                var _logger = services.BuildServiceProvider().GetRequiredService<ILogger<CsRedisManager>>();
                _logger.LogInformation($"Redis已关闭，当前驱动为NullCache!!!");
                services.AddSingleton<IRedisManager, NullCache>();
            }

            return services;
        }
    }
}
