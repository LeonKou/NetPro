using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.IO;
using System.NetPro;
using System.Threading.Tasks;

namespace NetPro.Web.Api
{
    /// <summary>
    /// api执行时间监控
    /// </summary>
    public class BenchmarkActionFilter : IAsyncActionFilter
    {
        private readonly ILogger _logger;
        readonly NetProOption _config;
        readonly IWebHelper _webHelper;
        readonly IConfiguration _configuration;

        public BenchmarkActionFilter(ILogger<BenchmarkActionFilter> logger, NetProOption config, IWebHelper webHelper, IConfiguration configuration)
        {
            _logger = logger;
            _config = config;
            _webHelper = webHelper;
            _configuration = configuration;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            int requestWarning = _config.RequestWarningThreshold < 3 ? 3 : _config.RequestWarningThreshold;

            var stopWatch = new Stopwatch();
            stopWatch.Start();

            var executedContext = await next();

            stopWatch.Stop();
            try
            {
                //如果执行超过设定的阈值则记录日志
                long executeTime = stopWatch.ElapsedMilliseconds / 1000;
                string requestBodyText = string.Empty;
                var request = context.HttpContext.Request;
                var method = request.Method.ToUpper();
                var url = UriHelper.GetDisplayUrl(request);
                var macName = Environment.MachineName;
                var requestIp = _webHelper.GetCurrentIpAddress();

                if (executeTime >= requestWarning)
                {
                    if (method.Equals("GET", StringComparison.OrdinalIgnoreCase))
                    {
                        _logger.LogWarning("请求时间超过阈值.执行时间:{0}秒.请求url:{1},请求IP:{2},服务器名称:{3}",
                           stopWatch.ElapsedMilliseconds / 1000, url, requestIp, macName);//formate格式日志对{索引}有颜色支持
                    }
                    else
                    {
                        if (!request.Body.CanSeek)
                        {
                            request.EnableBuffering();
                        }
                        request.Body.Seek(0L, SeekOrigin.Begin);

                        StreamReader stream = new StreamReader(request.Body);
                        string body = await stream.ReadToEndAsync();

                        _logger.LogWarning("请求时间超过阈值.执行时间:{0}秒.请求url:{1},请求Body:{2},请求IP:{3},服务器名称:{4}",
                            stopWatch.ElapsedMilliseconds / 1000, url, body, requestIp, macName);//formate格式日志对{索引}有颜色支持
                    }
                }
                if (!context.HttpContext.Response.Headers.ContainsKey("x-time-elapsed"))
                {
                    context.HttpContext.Response.Headers.Add(
                        "x-time-elapsed",
                        stopWatch.Elapsed.ToString());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "api执行时间监控日志异常！");
            }
        }
    }
}
