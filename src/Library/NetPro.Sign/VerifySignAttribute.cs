using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NetPro.Sign;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NetPro.Web.Core.Filters
{
    /// <summary>
    /// 验签特性
    /// </summary>
    /// <remarks>特性方式继承自动生效</remarks>
    public class VerifySignAttribute : ActionFilterAttribute
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IOperationFilter _verifySignCommon;
        private readonly VerifySignOption _verifySignOption;
        public VerifySignAttribute()
        {
            _logger = IoC.Resolve<ILogger<VerifySignAttribute>>();
            Order = 1;
            _configuration = IoC.Resolve<IConfiguration>();
            _verifySignCommon = IoC.Resolve<IOperationFilter>();
            _verifySignOption = IoC.Resolve<VerifySignOption>();
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var descriptor = (Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor)context.ActionDescriptor;
            var attributeController = (IgnoreSignAttribute)descriptor.ControllerTypeInfo.GetCustomAttributes(typeof(IgnoreSignAttribute), true).FirstOrDefault();
            if (attributeController != null)
                goto gotoNext;

            var attribute = (IgnoreSignAttribute)descriptor.MethodInfo.GetCustomAttributes(typeof(IgnoreSignAttribute), true).FirstOrDefault();
            if (attribute != null)
                goto gotoNext;

            if (!GetSignValue(context.HttpContext.Request))
            {
                SignCommon.BuildErrorJson(context);
                return;
            }
            else
                goto gotoNext;

            gotoNext:
            base.OnActionExecuting(context);
        }

        private bool GetSignValue(HttpRequest request)
        {
            try
            {
                var queryDic = request.Query.ToDictionary(s => s.Key, s => s.Value);
                var commonParameters = _verifySignOption.CommonParameters;
                if (!queryDic.ContainsKey(commonParameters.TimestampName) || !queryDic.ContainsKey(commonParameters.AppIdName) || !queryDic.ContainsKey(commonParameters.SignName))
                {
                    _logger.LogError("url参数中未找到签名所需参数[timestamp];[appid]或[sign]");
                    return false;
                }

                string signMethod = "hmac-sha256";
                if (queryDic.ContainsKey("signmethod"))
                {
                    signMethod = queryDic["signmethod"];
                }

                var timestampStr = queryDic[commonParameters.TimestampName];
                if (!long.TryParse(timestampStr, out long timestamp) || !CheckTime(timestamp))
                {
                    _logger.LogError($"{timestampStr}时间戳已过期");
                    return false;
                }

                var appIdString = queryDic[commonParameters.AppIdName].ToString();
                if (string.IsNullOrEmpty(appIdString))
                {
                    _logger.LogError(@"The request parameter is missing the Ak/Sk appID parameter
                                          VerifySign:{
                                            AppSecret:{
                                            [AppId]:[Secret]
                                                      }}");
                    return false;
                }

                var signvalue = queryDic[commonParameters.SignName].ToString();
                queryDic.Remove(commonParameters.SignName);

                var bodyValue = SignCommon.ReadAsString(request);
                if (!string.IsNullOrEmpty(bodyValue) && !"null".Equals(bodyValue))
                {
                    bodyValue = Regex.Replace(bodyValue, @"\s(?=([^""]*""[^""]*"")*[^""]*$)", string.Empty);
                    //bodyValue = bodyValue.Replace("\r\n", "").Replace(" : ", ":").Replace("\n  ", "").Replace("\n", "").Replace(": ", ":").Replace(", ", ",");

                    queryDic.Add("body", bodyValue);
                }
                var dicOrder = queryDic.OrderBy(s => s.Key, StringComparer.Ordinal).ToList();

                StringBuilder requestStr = new StringBuilder();
                StringBuilder logString = new StringBuilder();

                for (int i = 0; i < dicOrder.Count(); i++)
                {
                    requestStr.Append($"{dicOrder[i].Key}{dicOrder[i].Value}");

                    if (i == dicOrder.Count() - 1)
                    {
                        logString.Append($"{dicOrder[i].Key}={dicOrder[i].Value}");
                    }

                    else
                    {
                        logString.Append($"{dicOrder[i].Key}={dicOrder[i].Value}&");
                    }
                }

                var utf8Request = SignCommon.GetUtf8(requestStr.ToString());

                var result = _verifySignCommon.GetSignhHash(utf8Request, _verifySignCommon.GetSignSecret(appIdString), signMethod);
                if (_verifySignOption.IsDebug)
                {
                    _logger.LogInformation($"请求接口地址：{request.Path}");
                    _logger.LogInformation($"拼装排序后的值{logString}");
                    _logger.LogInformation($"拼装排序后的值{logString}");
                    _logger.LogInformation($"摘要计算后的值：{result}");
                    _logger.LogInformation($"摘要比对： {result}----{signvalue }");
                }
                else if (signvalue != result)
                {
                    _logger.LogWarning(@$"摘要被篡改：[iphide]----{signvalue }
                                            查看详情，请设置VerifySignOption节点的IsDebug为true");
                }
                return signvalue == result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "签名异常");
                return false;
            }
        }

        private bool CheckTime(long requestTime)
        {
            long unixSeconds = DateTimeOffset.Now.ToUnixTimeSeconds();

            if (requestTime + _verifySignOption.ExpireSeconds - unixSeconds < 0)
            {
                return false;
            }
            return true;
        }
    }
}
