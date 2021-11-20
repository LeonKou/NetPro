using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace NetPro.Checker
{
    public static class CheckMiddlewareHandler
    {
        private static readonly string DEFAULT_CONTENT_TYPE = "application/json";

        /// <summary>
        /// inclued: EnvCheck ;InfoCheck
        /// </summary>
        /// <param name="app"></param>
        /// <param name="openIp">Whether external access is allowed</param>
        /// <param name="envPath"></param>
        /// <param name="infoPath"></param>
        public static void UseCheck(this IApplicationBuilder app, bool openIp = true, string envPath = "/env", string infoPath = "/info")
        {
            app.UseEnvCheck(openIp, envPath);
            app.UseInfoCheck(openIp, infoPath);
            //app.UseHealthCheck("/check");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="app"></param>
        /// <param name="openIp"></param>
        /// <param name="path"></param>
        public static void UseEnvCheck(this IApplicationBuilder app, bool openIp = true, string path = "/env")
        {
            var config = app.ApplicationServices.GetService<IConfiguration>();
            app.Map(path, s =>
            {
                s.Run(async context =>
                {
                    var remoteIp = context.Connection.RemoteIpAddress;

                    if (IPAddress.IsLoopback(remoteIp) || openIp)
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
        /// <param name="openIp"></param>
        /// <param name="path"></param>
        public static void UseInfoCheck(this IApplicationBuilder app, bool openIp = true, string path = "/info")
        {
            var config = app.ApplicationServices.GetService<IConfiguration>();
            app.Map(path, s =>
            {
                s.Run(async context =>
                {
                    var remoteIp = context.Connection.RemoteIpAddress;

                    if (IPAddress.IsLoopback(remoteIp) || openIp)
                    {
                        var configuration = app.ApplicationServices.GetService(typeof(IConfiguration)) as IConfiguration;
                        var info = AppInfo.GetAppInfo(configuration);
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
            var config = app.ApplicationServices.GetService<IConfiguration>();
            app.Map(path, s =>
            {
                s.Run(async context =>
                {
                    var remoteIp = context.Connection.RemoteIpAddress;

                    if (IPAddress.IsLoopback(remoteIp) || config.GetValue<bool>($"{nameof(CheckOption)}:OpenIp"))
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
