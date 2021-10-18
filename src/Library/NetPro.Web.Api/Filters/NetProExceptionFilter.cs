using EnumsNET;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NetPro.Core.Configuration;
using NetPro.Core.Consts;
using NetPro.Core.Infrastructure.Attributes;
using Serilog;
using Serilog.Events;
using System;
using System.Net;

namespace NetPro.Web.Api
{
    /// <summary>
    /// 全局异常捕获过滤器,异常由中间件完成
    /// </summary>
    [Obsolete("废弃")]
    public class NetProExceptionFilter : IExceptionFilter
    {
        private readonly ILogger _logger;
        readonly IWebHelper _webHelper;
        readonly NetProOption _config;

        public NetProExceptionFilter(ILogger logger, IWebHelper webHelper, NetProOption config)
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
        public void OnException(ExceptionContext context)
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
            _logger.Write(eventLevel, exception, "{0}异常.errorCode:{1},errorMsg:{2},请求url:{3},请求Body:{4},请求IP:{5},服务器名称:{6}", appName, (int)errorCode, errorMsg, url, requestBodyText, requestIp, macName);
           
            //自定义异常返回
            context.Result = errorMsg.ToErrorActionResult((int)errorCode);
            context.HttpContext.Response.StatusCode = (int)statusCode;
            context.ExceptionHandled = true;
            return;
        }
    }
}
