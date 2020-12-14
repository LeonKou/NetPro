using System;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core;
using StackExchange.Redis.Extensions.Core.Abstractions;
using StackExchange.Redis.Extensions.Core.Configuration;
using StackExchange.Redis.Extensions.Core.Implementations;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// A set of extension methods that help you to confire StackExchangeRedisExtensions into your dependency injection
    /// </summary>
    public static class IServiceCollectionExtensions
    {
        /// <summary>
        /// 增加StackExchange.Redis服务
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="redisConfiguration">The redis configration.</param>
        /// <typeparam name="T">The typof of serializer. <see cref="ISerializer" />.</typeparam>
        public static IServiceCollection AddStackExchangeRedisExtensions<T>(
            this IServiceCollection services,
            RedisConfiguration redisConfiguration)
            where T : class, ISerializer, new()
        {
            return services.AddStackExchangeRedisExtensions<T>(sp => redisConfiguration);
        }

        /// <summary>
        /// 增加StackExchange.Redis服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="redisConfigurationFactory"></param>
        /// <typeparam name="T"> <see cref="ISerializer" />.</typeparam>
        public static IServiceCollection AddStackExchangeRedisExtensions<T>(
            this IServiceCollection services,
            Func<IServiceProvider, RedisConfiguration> redisConfigurationFactory)
            where T : class, ISerializer, new()
        {
            services.AddSingleton<IRedisCacheClient, RedisCacheClient>();
            services.AddSingleton<IRedisCacheConnectionPoolManager, RedisCacheConnectionPoolManager>();
            services.AddSingleton<ISerializer, T>();

            services.AddSingleton((provider) =>
            {
                return provider.GetRequiredService<IRedisCacheClient>().GetDbFromConfiguration();
            });

            services.AddMemoryCache();
            services.AddSingleton(redisConfigurationFactory);

            return services;
        }

        /// <summary>
        /// 增加StackExchange.Redis服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddStackExchangeRedisExtensions<T>(this IServiceCollection services, IConfiguration configuration) where T : class, ISerializer, new()
        {
            var option = configuration.GetSection(nameof(RedisConfiguration)).Get<RedisConfiguration>();
            if (option == null)
            {
                throw new ArgumentNullException(nameof(RedisConfiguration), $"未检测到NetPro.StackExchange.Redis配置节点{nameof(RedisConfiguration)}");
            }
            option.AbortOnConnectFail = false;
            option.ServerEnumerationStrategy = new ServerEnumerationStrategy()
            {
                Mode = ServerEnumerationStrategy.ModeOptions.All,
                TargetRole = ServerEnumerationStrategy.TargetRoleOptions.Any,
                UnreachableServerAction = ServerEnumerationStrategy.UnreachableServerActionOptions.Throw
            };

            return services.AddStackExchangeRedisExtensions<T>(sp => option);
        }
    }
}
