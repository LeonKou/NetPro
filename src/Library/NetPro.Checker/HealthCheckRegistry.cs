using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace NetPro.Checker
{
    /// <summary>
    /// Registry for health checks
    /// </summary>
    public class HealthCheckRegistry
    {
        public struct HealthStatus
        {
            /// <summary>
            /// Flag indicating whether any checks are registered
            /// </summary>
            //[JsonIgnore]
            public readonly bool HasRegisteredChecks;

            /// <summary>
            /// Whether or not all health checks have passed
            /// </summary>
            public readonly bool IsHealthy;

            /// <summary>
            /// Array containing result of each registered health check
            /// </summary>
            public readonly HealthCheck.Result[] Results;

            public HealthStatus(IEnumerable<HealthCheck.Result> results)
            {
                Results = results.ToArray();
                IsHealthy = Results.All(r => r.Check.IsHealthy);
                HasRegisteredChecks = Results.Length > 0;
            }
        }

        private static readonly ConcurrentDictionary<string, HealthCheck> Checks = new ConcurrentDictionary<string, HealthCheck>();

        public static void RegisterHealthCheck(string name, Action check)
        {
            RegisterHealthCheck(new HealthCheck(name, check));
        }

        public static void RegisterHealthCheck(string name, Func<string> check)
        {
            RegisterHealthCheck(new HealthCheck(name, check));
        }

        public static void RegisterHealthCheck(string name, Func<HealthResponse> check)
        {
            RegisterHealthCheck(new HealthCheck(name, check));
        }

        public static void RegisterHealthCheck(HealthCheck healthCheck)
        {
            Checks.TryAdd(healthCheck.Name, healthCheck);
        }

        public static HealthStatus GetStatus()
        {
            var results = Checks.Values.Select(v => v.Execute()).OrderBy(r => r.Name);
            return new HealthStatus(results);
        }

        public static void UnregisterAllHealthChecks()
        {
            Checks.Clear();
        }

    }
}
