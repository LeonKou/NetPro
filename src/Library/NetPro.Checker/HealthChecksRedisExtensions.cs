using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using StackExchange.Redis;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NetPro.Checker
{
    public static class HealthChecksRedisExtensions
    {
        private static readonly string NAME = $"redis-{Guid.NewGuid()}";

        /// <summary>
        /// Add a health check for Redis services.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="redisConnectionString"></param>
        /// <param name="name"></param>
        /// <param name="failureStatus"></param>
        /// <param name="tags"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static IHealthChecksBuilder AddRedis(this IHealthChecksBuilder builder, string redisConnectionString, string name = default, HealthStatus? failureStatus = default, IEnumerable<string> tags = default, TimeSpan? timeout = default)
        {
            return builder.Add(new HealthCheckRegistration(
               name ?? NAME,
               sp => new RedisHealthCheck(redisConnectionString),
               failureStatus,
               tags,
               timeout));
        }
    }

    public class RedisHealthCheck : IHealthCheck
    {
        private static readonly ConcurrentDictionary<string, ConnectionMultiplexer> Connections = new ConcurrentDictionary<string, ConnectionMultiplexer>();
        private readonly string _redisConnectionString;

        public RedisHealthCheck(string redisConnectionString)
        {
            _redisConnectionString = redisConnectionString ?? throw new ArgumentNullException(nameof(redisConnectionString));
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                if (!Connections.TryGetValue(_redisConnectionString, out ConnectionMultiplexer connection))
                {
                    connection = await ConnectionMultiplexer.ConnectAsync(_redisConnectionString);

                    if (!Connections.TryAdd(_redisConnectionString, connection))
                    {
                        // Dispose new connection which we just created, because we don't need it.
                        connection.Dispose();
                        connection = Connections[_redisConnectionString];
                    }
                }

                await connection.GetDatabase()
                    .PingAsync();

                if (!connection.IsConnected)
                    return new HealthCheckResult(context.Registration.FailureStatus, $"redis IsConnected is false");

                return HealthCheckResult.Healthy();
            }
            catch (Exception ex)
            {
                return new HealthCheckResult(context.Registration.FailureStatus, exception: ex);
            }
        }
    }
}
