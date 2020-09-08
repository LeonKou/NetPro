using NetPro.Core;
using NetPro.Core.Configuration;
using NetPro.Web.Core.Helpers;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace NetPro.Web.Core.Filters
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

        public BenchmarkActionFilter(ILogger logger, NetProOption config, IWebHelper webHelper,  IConfiguration configuration)
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
                var bodyText = ActionFilterHelper.GetRequestBodyText(request, _configuration.GetValue<string>("AliyunLogRule:Regex"));
                if (executeTime >= requestWarning)
                {
                    _logger.Warning("请求时间超过阈值.执行时间:{0}秒.请求url:{1},请求Body:{2},请求IP:{3},服务器名称:{4}",
                        stopWatch.ElapsedMilliseconds / 1000, url, bodyText, requestIp, macName);
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
                _logger.Error(ex, "api执行时间监控日志异常！");
            }
        }
    }
}
