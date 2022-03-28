using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetTools;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace NetPro.Checker
{
    public static class CheckMiddlewareHandler
    {
        private static readonly string DEFAULT_CONTENT_TYPE = "application/json";

        /// <summary>
        /// inclued: EnvCheck ;InfoCheck
        /// </summary>
        /// <param name="app"></param>
        /// <param name="envPath"></param>
        /// <param name="infoPath"></param>
        /// <param name="health"></param>
        public static void UseCheck(this IApplicationBuilder app, string envPath = "/env", string infoPath = "/info", string health = "/check")
        {
            app.UseEnvCheck(envPath);
            app.UseInfoCheck(infoPath);
            app.UseHealthCheck(health);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="app"></param>
        /// <param name="path"></param>
        public static void UseEnvCheck(this IApplicationBuilder app, string path = "/env")
        {
            var config = app.ApplicationServices.GetService<IConfiguration>();
            app.Map(path, s =>
            {
                s.Run(async context =>
                {
                    //ip可篡改，只作为基础限制，实际建议nginx处配置路由映射的方式限制
                    var remoteIp = context.Connection.RemoteIpAddress;
                    var rangeA = IPAddressRange.Parse("192.168.0.0 - 192.168.255.255");
                    var rangeB = IPAddressRange.Parse("172.16.0.0 - 172.31.255.255");
                    var rangeC = IPAddressRange.Parse("10.0.0.0 - 10.255.255.255");
                    if (IPAddress.IsLoopback(remoteIp) || rangeA.Contains(remoteIp) || rangeB.Contains(remoteIp) || rangeC.Contains(remoteIp))
                    {
                        var env = AppEnvironment.GetAppEnvironment();
                        context.Response.ContentType = DEFAULT_CONTENT_TYPE;
                        await context.Response.WriteAsync(Serialize(env));
                    }
                    else
                    {
                        context.Response.StatusCode = 403;
                        context.Response.ContentType = "application/html";
                        await context.Response.WriteAsync("<font size=\"7\">403</font><br/>");
                    }
                });
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="app"></param>
        /// <param name="path"></param>
        public static void UseInfoCheck(this IApplicationBuilder app, string path = "/info")
        {
            var config = app.ApplicationServices.GetService<IConfiguration>();
            app.Map(path, s =>
            {
                s.Run(async context =>
                {
                    var remoteIp = context.Connection.RemoteIpAddress;
                    var rangeA = IPAddressRange.Parse("192.168.0.0 - 192.168.255.255");
                    var rangeB = IPAddressRange.Parse("172.16.0.0 - 172.31.255.255");
                    var rangeC = IPAddressRange.Parse("10.0.0.0 - 10.255.255.255");
                    if (IPAddress.IsLoopback(remoteIp) || rangeA.Contains(remoteIp) || rangeB.Contains(remoteIp) || rangeC.Contains(remoteIp))
                    {
                        var info = AppInfo.GetAppInfo(config);
                        info.RequestHeaders = context.Request.Headers.ToDictionary(kv => kv.Key, kv => kv.Value.First());
                        //context.Response.Headers["Content-Type"] = "application/json";
                        context.Response.ContentType = DEFAULT_CONTENT_TYPE;
                        await context.Response.WriteAsync(Serialize(info));
                    }
                    else
                    {
                        context.Response.StatusCode = 403;
                        context.Response.ContentType = "application/html";
                        await context.Response.WriteAsync("<font size=\"7\">403</font><br/>");
                    }
                });
            });
        }

        [Obsolete("recommended to use IApplicationBuilder.UseCheck")]
        public static void UseHealthCheck(this IApplicationBuilder app, string path = "/health")
        {
            app.Map(path, s =>
            {
                s.Run(async context =>
                {
                    var remoteIp = context.Connection.RemoteIpAddress;
                    var rangeA = IPAddressRange.Parse("192.168.0.0 - 192.168.255.255");
                    var rangeB = IPAddressRange.Parse("172.16.0.0 - 172.31.255.255");
                    var rangeC = IPAddressRange.Parse("10.0.0.0 - 10.255.255.255");
                    if (IPAddress.IsLoopback(remoteIp) || rangeA.Contains(remoteIp) || rangeB.Contains(remoteIp) || rangeC.Contains(remoteIp))
                    {
                        HealthCheckRegistry.HealthStatus status = await Task.Run(() => HealthCheckRegistry.GetStatus());

                        if (!status.IsHealthy)
                        {
                            // Return a service unavailable status code if any of the checks fail
                            context.Response.StatusCode = 503;
                        }
                        context.Response.ContentType = DEFAULT_CONTENT_TYPE;
                        await context.Response.WriteAsync(JsonConvert.SerializeObject(status));
                    }
                    else
                    {
                        context.Response.StatusCode = 403;
                        context.Response.ContentType = "application/html";
                        await context.Response.WriteAsync("<font size=\"7\">403</font><br/>");
                    }
                });
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="report"></param>
        /// <returns></returns>
        public static async Task WriteHealthCheckUiResponse(HttpContext httpContext, HealthReport report)
        {
            httpContext.Response.ContentType = DEFAULT_CONTENT_TYPE;
            if (report != null)
            {
                await httpContext.Response.WriteAsync(CreateFrom(report));
            }
            else
            {
                await httpContext.Response.WriteAsync(Serialize(new { Status = HealthStatus.Degraded.ToString() }));
            }
        }

        private static string CreateFrom(HealthReport report)
        {
            if (report == null) return string.Empty;
            var result = new Dictionary<string, CustomerHealthReport>();
            foreach (var item in report.Entries)
            {
                var entry = new CustomerHealthReport
                {
                    Data = item.Value.Data,
                    Description = item.Value.Description,
                    Duration = item.Value.Duration,
                    Status = item.Value.Status.ToString()
                };

                if (item.Value.Exception != null)
                {
                    var message = item.Value.Exception?
                        .Message;

                    entry.Exception = message;
                    entry.Description = item.Value.Description ?? message;
                }
                result.Add(item.Key, entry);
            }
            return Serialize(new { Status = report.Status.ToString(), result });
        }
        private static string Serialize<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj, new JsonSerializerSettings { Formatting = Formatting.Indented });
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class CustomerHealthReport
    {
        /// <summary>
        /// 
        /// </summary>
        public IReadOnlyDictionary<string, object> Data { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public TimeSpan Duration { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Exception { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Status { get; set; }
    }
}
