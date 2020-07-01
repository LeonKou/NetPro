using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace NetPro.Sign
{
    public class VerifySignFilter : IAsyncActionFilter
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IOperationFilter _verifySignCommon;
        private readonly VerifySignOption _verifySignOption;

        public VerifySignFilter(ILogger<VerifySignFilter> logger, IConfiguration configuration, IOperationFilter verifySignCommon, VerifySignOption verifySignOption)
        {
            _logger = logger;
            _configuration = configuration;
            _verifySignCommon = verifySignCommon;
            _verifySignOption = verifySignOption;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!_verifySignOption.Enable || _verifySignOption.Scheme?.ToLower() != "attribute")
            {
                goto gotoNext;
            }
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
                await Task.CompletedTask;
                return;
            }
            else
                goto gotoNext;

            gotoNext:
            await next();
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
                    var dict = JsonSerializer.Deserialize<Dictionary<string, string>>(bodyValue);
                    foreach (var item in dict)
                    {
                        queryDic.Add(item.Key, item.Value);
                        if (_verifySignOption.IsDebug)
                            _logger.LogInformation($"字段:{item.Key}--值:{item.Value}");
                    }
                }
                var dicOrder = queryDic.OrderBy(s => s.Key, StringComparer.Ordinal).ToList();

                StringBuilder requestStr = new StringBuilder();
                for (int i = 0; i < dicOrder.Count(); i++)
                {
                    if (i == dicOrder.Count() - 1)
                        requestStr.Append($"{dicOrder[i].Key}={dicOrder[i].Value}");
                    else
                        requestStr.Append($"{dicOrder[i].Key}={dicOrder[i].Value}&");
                }

                var utf8Request = SignCommon.GetUtf8(requestStr.ToString());

                var result = _verifySignCommon.GetSignhHash(utf8Request, _verifySignCommon.GetSignSecret(appIdString));
                if (_verifySignOption.IsDebug)
                {
                    _logger.LogInformation($"拼装排序后的值{request.Path}");
                    _logger.LogInformation($"拼装排序后的值{requestStr}");
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
