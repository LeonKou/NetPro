using App.Metrics.Health;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace NetPro.Web.Core.HealthChecks
{
    /// <summary>
    /// 数据库健康检查
    /// </summary>
   public class DBHealthCheck : HealthCheck
    {
        public DBHealthCheck() : base("Custom health checks")
        {

        }

        protected override ValueTask<HealthCheckResult> CheckAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            if (DateTime.UtcNow.Second <= 20)
            {
                return new ValueTask<HealthCheckResult>(HealthCheckResult.Degraded());
            }

            if (DateTime.UtcNow.Second >= 40)
            {
                return new ValueTask<HealthCheckResult>(HealthCheckResult.Unhealthy());
            }

            return new ValueTask<HealthCheckResult>(HealthCheckResult.Healthy());
        }
    }
}
