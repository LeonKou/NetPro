using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System.Collections.Generic;

namespace NetPro.RedisManager
{
	public static class RedisServiceExtensions
	{
		public static IServiceCollection AddRedisManager(this IServiceCollection services, IConfiguration configuration)
		{
			var option= configuration.GetSection(nameof(RedisCacheOption)).Get<RedisCacheOption>();
			
			services.AddRedisManager(option);
			return services;
		}

		public static IServiceCollection AddRedisManager(this IServiceCollection services, RedisCacheOption redisCacheOption)
		{																	 			
			var redisCacheComponent = redisCacheOption?.RedisComponent ?? RedisCacheComponentEnum.NullRedis;

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

		public static IServiceCollection AddCsRedis(this IServiceCollection services, RedisCacheOption option)
		{
			services.AddSingleton(option);
			List<string> csredisConns = new List<string>();
			string password = option.Password;
			int defaultDb = option.Database;
			string ssl = option.SslHost;
			string keyPrefix = option.DefaultCustomKey;
			int writeBuffer = 10240;
			int poolsize = 10;
			foreach (var e in option.Endpoints)
			{
				string server = e.Host;
				int port = e.Port;
				if (string.IsNullOrWhiteSpace(server) || port <= 0) { continue; }
				csredisConns.Add($"{server}:{port},password={password},defaultDatabase={defaultDb},poolsize={poolsize},ssl={ssl},writeBuffer={writeBuffer},prefix={keyPrefix}");
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
			
			RedisHelper.Initialization(csredis);
			services.AddScoped<IRedisManager, CsRedisManager>();
			return services;
		}

		public static IServiceCollection AddCsRedis(this IServiceCollection services, IConfiguration config)
		{
			var option = new RedisCacheOption(config);
			services.AddCsRedis(option);
			return services;
		}

		public static IServiceCollection AddStackExchangeRedis(this IServiceCollection services, IConfiguration config)
		{
			var redisOption = new RedisCacheOption(config);

			services.AddStackExchangeRedis(redisOption);
			return services;
		}

		public static IServiceCollection AddStackExchangeRedis(this IServiceCollection services, RedisCacheOption redisOption)
		{
			services.AddSingleton(redisOption);
			var configurationOptions = new ConfigurationOptions
			{
				ConnectTimeout = redisOption.ConnectionTimeout,
				Password = redisOption.Password,
				Ssl = redisOption.IsSsl,
				SslHost = redisOption.SslHost,
				AbortOnConnectFail = false,
			};

			foreach (var endpoint in redisOption.Endpoints)
			{
				configurationOptions.EndPoints.Add(endpoint.Host, endpoint.Port);
			}
			services.Add(ServiceDescriptor.Singleton(ConnectionMultiplexer.Connect(configurationOptions)));
			services.AddSingleton<IRedisManager, StackExchangeRedisManager>();
			return services;
		}
	}
}
