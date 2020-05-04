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
		private static readonly string NAME = "redis";

		/// <summary>
		/// Add a health check for Redis services.
		/// </summary>
		/// <param name="builder">The <see cref="IHealthChecksBuilder"/>.</param>
		/// <param name="redisConnectionString">The Redis connection string to be used.</param>
		/// <param name="name">The health check name. Optional. If <c>null</c> the type name 'redis' will be used for the name.</param>
		/// <param name="failureStatus">
		/// The <see cref="HealthStatus"/> that should be reported when the health check fails. Optional. If <c>null</c> then
		/// the default status of <see cref="HealthStatus.Unhealthy"/> will be reported.
		/// </param>
		/// <param name="tags">A list of tags that can be used to filter sets of health checks. Optional.</param>
		/// <param name="timeout">An optional System.TimeSpan representing the timeout of the check.</param>
		/// <returns>The <see cref="IHealthChecksBuilder"/>.</returns></param>
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
