using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace NetPro.Checker
{
    /// <summary>
    /// url health check
    /// </summary>
    public static class HealthChecksUrlExtensions
    {
        private static readonly string NAME = "urlcheck";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="urls"></param>
        /// <param name="succeedtatusCodes">default is HttpStatusCode.OK </param>
        /// <param name="name"></param>
        /// <param name="failureStatus"></param>
        /// <param name="tags"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static IHealthChecksBuilder AddUrl(this IHealthChecksBuilder builder, List<string> urls, List<HttpStatusCode> succeedtatusCodes = null, string name = default, HealthStatus? failureStatus = default, IEnumerable<string> tags = default, TimeSpan? timeout = default)
        {
            if (succeedtatusCodes == null)
            {
                succeedtatusCodes = new List<HttpStatusCode>() { HttpStatusCode.OK };
            }
            return builder.Add(new HealthCheckRegistration(
                name ?? NAME,
                sp => new UrlHealthCheck(urls, succeedtatusCodes),
                failureStatus,
                tags,
                timeout));
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class UrlHealthCheck : IHealthCheck
    {
        private readonly List<string> _urls;
        private static HttpClient _httpclient;
        private readonly List<HttpStatusCode> _statusCodes;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="urls"></param>
        /// <param name="statusCodes"></param>
        public UrlHealthCheck(List<string> urls, List<HttpStatusCode> statusCodes)
        {
            _httpclient = new HttpClient();
            _urls = urls;
            _statusCodes = statusCodes;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            List<string> message = new List<string>();
            List<string> exception = new List<string>();
            foreach (var item in _urls)
            {
                try
                {
                    var result = await _httpclient.GetAsync(item);
                    if (!_statusCodes.Contains(result.StatusCode))
                    {
                        message.Add($"[{item}] StatusCode is `[{result.StatusCode}]`，request exception");
                    }
                }
                catch (Exception ex)
                {
                    exception.Add($"[{item}]Exception is ----->[{ex.Message}]");
                }
            }
            if (exception.Any())
            {
                return new HealthCheckResult(context.Registration.FailureStatus, exception: new Exception(string.Join(',', exception)));
            }
            if (message.Any())
            {
                return new HealthCheckResult(context.Registration.FailureStatus, string.Join("--------", message));
            }
            return HealthCheckResult.Healthy();
        }
    }
}