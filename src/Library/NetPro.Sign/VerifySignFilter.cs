using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
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
            if (!_verifySignOption.Enable || _verifySignOption.Scheme?.ToLower() != "global")
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

            var result = await GetSignValue(context.HttpContext);
            if (_verifySignOption.IsForce && !result.Item1)
            {
                SignCommon.BuildErrorJson(context, result.Item2);
                await Task.CompletedTask;
                return;
            }
            else
                goto gotoNext;

            gotoNext:
            await next();
        }

        private async Task<Tuple<bool, string>> GetSignValue(HttpContext request)
        {
            try
            {
                var convertedDictionatry = request.Request.Query.ToDictionary(s => s.Key, s => s.Value);
                var queryDic = new Dictionary<string, string>();
                foreach (var item in convertedDictionatry)
                {
                    queryDic.Add(item.Key.ToLower(), item.Value);
                }
                var commonParameters = _verifySignOption.CommonParameters;
                if (!queryDic.ContainsKey(commonParameters.TimestampName) || !queryDic.ContainsKey(commonParameters.AppIdName) || !queryDic.ContainsKey(commonParameters.SignName))
                {
                    _logger.LogError("url参数中未找到签名所需参数[timestamp];[appid]或[sign]");
                    return Tuple.Create<bool, string>(false, "签名参数缺失");
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
                    return Tuple.Create<bool, string>(false, "请校准客户端时间后再试");
                }

                var appIdString = queryDic[commonParameters.AppIdName].ToString();
                if (string.IsNullOrEmpty(appIdString))
                {
                    _logger.LogError(@"The request parameter is missing the Ak/Sk appID parameter
                                          VerifySign:{
                                            AppSecret:{
                                            [AppId]:[Secret]
                                                      }}");
                    return Tuple.Create<bool, string>(false, "服务异常，AppIdName未配置");
                }

                var signvalue = queryDic[commonParameters.SignName].ToString();
                queryDic.Remove(commonParameters.SignName);

                var bodyValue = await SignCommon.ReadAsStringAsync(request);
                if (!string.IsNullOrEmpty(bodyValue) && !"null".Equals(bodyValue))
                {
                    bodyValue = Regex.Replace(bodyValue, @"\s(?=([^""]*""[^""]*"")*[^""]*$)", string.Empty);

                    bodyValue = bodyValue.Replace("\r\n", "").Replace(" : ", ":").Replace("\n  ", "").Replace("\n", "").Replace(": ", ":").Replace(", ", ",");

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
                    _logger.LogInformation($"请求接口地址：{request.Request.Path}");
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

                if (signvalue == result)
                {
                    return Tuple.Create<bool, string>(true, "签名通过");
                }
                return Tuple.Create<bool, string>(false, "签名异常,请求非法");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "签名异常");
                return Tuple.Create<bool, string>(false, "签名异常");
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
