using NetPro.Core;
using NetPro.Core.Configuration;
using NetPro.Core.Consts;
using NetPro.Core.Infrastructure.Attributes;
using NetPro.Utility;
using NetPro.Web.Core.Helpers;
using EnumsNET;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;
using Serilog.Events;
using System;
using System.Net;
using System.Threading.Tasks;

namespace NetPro.Web.Core.Filters
{
    /// <summary>
    /// 全局异常捕获过滤器
    /// </summary>
    public class NetProExceptionFilter : IAsyncExceptionFilter
    {
        private readonly ILogger _logger;
        readonly IWebHelper _webHelper;
        readonly NetProOption _config;

        public NetProExceptionFilter(ILogger logger,IWebHelper webHelper, NetProOption config)
        {
            _logger = logger;
            _webHelper = webHelper;
            _config = config;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task OnExceptionAsync(ExceptionContext context)
        {
            string requestBodyText = string.Empty;
            var request = context.HttpContext.Request;
            var method = request.Method.ToUpper();
            var url = UriHelper.GetDisplayUrl(request);
            var macName = Environment.MachineName;
            var requestIp = _webHelper.GetCurrentIpAddress();
            var exception = context.Exception;

            #region 获取body参数
            if (!(exception is ViewModelStateValidException))
            {
                requestBodyText = ActionFilterHelper.GetRequestBodyText(request);
            }
            #endregion

            AppErrorCode errorCode = AppErrorCode.None;
            string requestId = string.Empty;
            var errorMsg = "程序访问异常,请稍后重试！";
            //请求bindModel验证失败时 请求body参数
            if (exception is ViewModelStateValidException)
            {
                requestBodyText = ((ViewModelStateValidException)exception).BindModelText;
            }

            HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
            //自定义异常,可预见性异常400，不可预见性异常500
            if (exception is NetProException)
            {
                var NetProEx = (NetProException)context.Exception;
                var exErrorCode = NetProEx.ErrorCode;
                errorMsg = NetProEx.Message;
                //判断errorCode是否为系统定义的错误代码
                if (((AppErrorCode)exErrorCode).IsValid())
                {
                    errorCode = (AppErrorCode)NetProEx.ErrorCode;
                }
                else
                {
                    errorCode = AppErrorCode.None;
                }
                requestId = NetProEx.RequestId;
                statusCode = HttpStatusCode.BadRequest;
            }
            var appName = "WebApi";
            if (_config != null)
            {
                appName = _config.ApplicationName;
            }
            //错误代码对应的日志级别
            NetProErrorLevel errorLevel = NetProErrorLevel.Error;
            var currentLevel = errorCode.GetAttributes()?.Get<ErrorCodeLevelAttribute>()?.Level;
            if (currentLevel.HasValue)
            {
                errorLevel = currentLevel.Value;
            }
            LogEventLevel eventLevel = LogEventLevel.Error;
            switch (errorLevel)
            {
                case NetProErrorLevel.Error:
                    eventLevel = LogEventLevel.Error;
                    break;
                case NetProErrorLevel.Fatal:
                    eventLevel = LogEventLevel.Fatal;
                    break;
                case NetProErrorLevel.Warning:
                    eventLevel = LogEventLevel.Warning;
                    exception = null;
                    break;
            }
            //写入日志系统
            _logger.Write(eventLevel, exception, "{0}异常.errorCode:{1},errorMsg:{2},请求url:{3},请求Body:{4},请求IP:{5},服务器名称:{6}", appName, errorCode.Value(), errorMsg, url, requestBodyText, requestIp, macName);
            //自定义异常返回
            if (_config.AppType == AppType.Api)
            {
                context.Result = errorMsg.ToErrorActionResult(errorCode.Value());
                context.HttpContext.Response.StatusCode = (int)statusCode;
            }
            else
            {
                string errorUrl = _config.ErrorUrl;
                if (string.IsNullOrWhiteSpace(errorUrl) || (!string.IsNullOrWhiteSpace(errorUrl) && errorUrl.Split('/').Length != 2))
                {
                    context.Result = new ContentResult() { Content = "您访问的页面出错!" };
                }
                else
                {
                    var array = errorUrl.Split('/');
                    context.Result = new RedirectToActionResult(array[1], array[0], new { error = exception.Message });
                }
                context.HttpContext.Response.StatusCode = (int)statusCode;
            }
            context.ExceptionHandled = true;
            return;
        }
    }
}
