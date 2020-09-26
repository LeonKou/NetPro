using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Driver;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;

namespace NetPro.Checker
{
    public static class HealthChecksMongodbExtensions
    {
        private static string NAME = "mongodb";

        /// <summary>
        /// Add a health check for MongoDb database that list all databases on the system.
        /// </summary>
        /// <param name="builder">The <see cref="IHealthChecksBuilder"/>.</param>
        /// <param name="mongodbConnectionString">The MongoDb connection string to be used.</param>
        /// <param name="name">The health check name. Optional. If <c>null</c> the type name 'mongodb' will be used for the name.</param>
        /// <param name="failureStatus">
        /// The <see cref="HealthStatus"/> that should be reported when the health check fails. Optional. If <c>null</c> then
        /// the default status of <see cref="HealthStatus.Unhealthy"/> will be reported.
        /// </param>
        /// <param name="tags">A list of tags that can be used to filter sets of health checks. Optional.</param>
        /// <param name="timeout">An optional System.TimeSpan representing the timeout of the check.</param>
        /// <returns>The <see cref="IHealthChecksBuilder"/>.</returns>
        public static IHealthChecksBuilder AddMongoDb(this IHealthChecksBuilder builder, string mongodbConnectionString, string name = default, HealthStatus? failureStatus = default, IEnumerable<string> tags = default, TimeSpan? timeout = default)
        {
            return builder.Add(new HealthCheckRegistration(
                name ?? NAME,
                sp => new MongoDbHealthCheck(mongodbConnectionString),
                failureStatus,
                tags,
                timeout));
        }
    }

    public class MongoDbHealthCheck : IHealthCheck
    {
        private static readonly ConcurrentDictionary<MongoClientSettings, MongoClient> MongoClient = new ConcurrentDictionary<MongoClientSettings, MongoClient>();
        private readonly MongoClientSettings _mongoClientSettings;
        private readonly string _specifiedDatabase;

        public MongoDbHealthCheck(string connectionString, string databaseName = default)
            : this(MongoClientSettings.FromUrl(MongoUrl.Create(connectionString)), databaseName)
        {
            if (databaseName == default)
            {
                _specifiedDatabase = MongoUrl.Create(connectionString)?.DatabaseName;
            }
        }

        public MongoDbHealthCheck(MongoClientSettings clientSettings, string databaseName = default)
        {
            _specifiedDatabase = databaseName;
            _mongoClientSettings = clientSettings;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var mongoClient = MongoClient.GetOrAdd(_mongoClientSettings, settings => new MongoClient(settings));
                var server = mongoClient.Settings.Server;

                Ping pingSender = new Ping();
                PingReply reply = pingSender.Send(server.Host, 5000);
                if (reply != null && reply.Status != IPStatus.Success)
                    return new HealthCheckResult(context.Registration.FailureStatus, $"ping {reply.Address} fail, status is{reply.Status}");

                if (!new System.Net.Sockets.TcpClient(server.Host, server.Port).Connected)
                    return new HealthCheckResult(context.Registration.FailureStatus, $" telnet {server.Host} {server.Port} fail");

                if (!string.IsNullOrEmpty(_specifiedDatabase))
                {
                    await (await mongoClient
                         .GetDatabase(_specifiedDatabase)
                         .ListCollectionsAsync(cancellationToken: cancellationToken)).FirstAsync(cancellationToken);
                }
                else
                {
                    await mongoClient
                        .ListDatabasesAsync(cancellationToken);
                }

                return HealthCheckResult.Healthy();
            }
            catch (Exception ex)
            {
                return new HealthCheckResult(context.Registration.FailureStatus, exception: ex);
            }
        }
    }
}
