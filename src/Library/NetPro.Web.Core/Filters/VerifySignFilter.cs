using NetPro.Core;
using NetPro.Core.Configuration;
using NetPro.Core.Consts;
using NetPro.Utility;
using NetPro.Utility.Helpers;
using NetPro.Web.Core.Helpers;
using NetPro.Web.Core.Middlewares;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NetPro.Web.Core.Filters
{
    /// <summary>
    /// 接口签名验证
    /// </summary>
    public class VerifySignAbandonedFilter : IAsyncActionFilter
    {
        private readonly ILogger _logger;
        readonly IConfiguration _configuration;

        public VerifySignAbandonedFilter(ILogger logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var descriptor = (Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor)context.ActionDescriptor;
            var attribute = (IgnoreSignAttribute)descriptor.MethodInfo.GetCustomAttributes(typeof(IgnoreSignAttribute), true).FirstOrDefault();
            if (attribute != null)
                goto gotoNext;

            string appId = null;
            try
            {
                var request = context.HttpContext.Request;
                var bodyText = TryPassJsonToQueryString(ActionFilterHelper.GetRequestBodyText(request).Trim());
                if (string.IsNullOrWhiteSpace(bodyText))
                {
                    BuildErrorJson(context);
                    return;
                }
                var allKeys = bodyText.TrimStart('?').Split('&').Select(a => new
                {
                    Key = a.Substring(0, a.IndexOf('=')).ToLower(),
                    Value = (object)a.Substring(a.IndexOf('=') + 1)
                }).ToDictionary(a => a.Key, a => a.Value);
                if (request.Headers != null)
                {
                    var headers = request.Headers.ToDictionary(a => a.Key.ToLower(), a => a.Value).Where(a => new[] { "appid", "sign" }.Contains(a.Key)).ToDictionary(a => a.Key, a => a.Value.ToString());
                    foreach (var header in headers)
                    {
                        if (allKeys.ContainsKey(header.Key))
                            allKeys[header.Key] = header.Value;
                        else
                            allKeys.Add(header.Key, header.Value);
                    }
                }
                if (!allKeys.ContainsKey("appid") || !allKeys.ContainsKey("sign"))
                {
                    BuildErrorJson(context);
                    return;
                }

                ////判断请求是否在有效期内
                ////获取过期时间（秒）
                //var expireSeconds = _configuration.GetValue<double>("ExpireSeconds", 0);
                ////请求过期时间
                //var requestExpireTime = new DateTime(ticks + TimeSpan.FromSeconds(expireSeconds).Ticks);
                ////如果请求过期时间小于当前时间则判定为过期
                //if (DateTime.Compare(requestExpireTime, DateTime.Now) == -1)
                //{
                //    BuildErrorJson(context);
                //    return;
                //}

                var secrets = new List<AppSecret>();
                _configuration.GetSection("Secrets").Bind(secrets);
                appId = (string)allKeys["appid"];
                var secret = secrets.FirstOrDefault(a => a.AppId == appId)?.Secret;
                var sign = SignHelper.GetSign(allKeys, secret);
                if (sign != (string)allKeys["sign"])
                {
                    BuildErrorJson(context);
                    return;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "【VerifySignFilter】error：" + ex.Message);
                BuildErrorJson(context, "签名验证失败!" + ex.Message);
                return;
            }

            gotoNext:
            await next();
        }

        private void BuildErrorJson(ActionExecutingContext context, string errorMsg = "签名验证失败!")
        {
            context.HttpContext.Response.StatusCode = 200;
            context.HttpContext.Response.ContentType = "application/json";
            context.Result = errorMsg.ToErrorActionResult(AppErrorCode.VerifySignFailt.Value());
        }

        private string TryPassJsonToQueryString(string json)
        {
            string queryString = json;
            if (string.IsNullOrEmpty(json)) return queryString;
            try
            {
                var dic = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                queryString = string.Join("&", dic.Select(a => Regex.Replace($"{a.Key}={a.Value}", "\\s*", string.Empty)));
            }
            catch
            {
                //_logger.Error(ex, "【TryPassJsonTo】error：" + ex.Message);
            }
            return queryString;
        }
    }
}
