using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NetPro.Sign;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NetPro.Web.Core.Filters
{
    /// <summary>
    /// 验签特性
    /// </summary>
    /// <remarks>特性方式继承自动生效</remarks>
    [Obsolete("过时的特性,签名只适合于中间件方式,用特新方式可能会导致其他中间件提前拦截导致无法命中签名")]
    public class VerifySignAttribute : ActionFilterAttribute
    {
        public VerifySignAttribute()
        {
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            IServiceProvider serviceProvider = context.HttpContext.RequestServices;
            var _logger = serviceProvider.GetRequiredService<ILogger<VerifySignAttribute>>();
            //var _configuration = serviceProvider.GetRequiredService<IConfiguration>();
            var _verifySignCommon = serviceProvider.GetService<IOperationFilter>();
            var _verifySignOption = serviceProvider.GetService<VerifySignOption>();

            //无配置默认关闭
            if (_verifySignOption == null || !_verifySignOption.Enabled)
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

            var result = await GetSignValue(context.HttpContext, _logger, _verifySignOption, _verifySignCommon);
            if (_verifySignOption.IsForce && !result.Item1)
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

        private async Task<Tuple<bool, string>> GetSignValue(HttpContext request, ILogger<VerifySignAttribute> _logger, VerifySignOption _verifySignOption, IOperationFilter _verifySignCommon)
        {
            try
            {
                var convertedDictionatry = request.Request.Query.ToDictionary(s => s.Key, s => s.Value);
                var queryDic = new Dictionary<string, string>();
                foreach (var item in convertedDictionatry)
                {
                    queryDic.Add(item.Key.ToLower(), item.Value);
                }
                var encryptEnum = EncryptEnum.Default;

                var commonParameters = _verifySignOption.CommonParameters;

                if (!queryDic.ContainsKey(commonParameters.TimestampName) || !queryDic.ContainsKey(commonParameters.AppIdName) || !queryDic.ContainsKey(commonParameters.SignName))
                {
                    _logger.LogWarning("方式：[VerifySign] url参数中未找到签名所需参数[timestamp];[appid];[EncryptFlag]或[sign]");
                    return Tuple.Create<bool, string>(false, "签名参数缺失");
                }

                if (queryDic.ContainsKey(commonParameters.EncryptFlag) && int.TryParse(queryDic[commonParameters.EncryptFlag].ToString(), out int encryptint))
                {
                    encryptEnum = (EncryptEnum)encryptint;
                }

                var timestampStr = queryDic[commonParameters.TimestampName];
                if (!long.TryParse(timestampStr, out long timestamp) || !CheckTime(timestamp, _verifySignOption))
                {
                    _logger.LogWarning($"方式：[VerifySign] {timestampStr}时间戳已过期");
                    return Tuple.Create<bool, string>(false, "请校准客户端时间后再试");
                }

                var appIdString = queryDic[commonParameters.AppIdName].ToString();
                if (string.IsNullOrEmpty(appIdString))
                {
                    _logger.LogWarning(@"方式：[VerifySign] The request parameter is missing the Ak/Sk appID parameter
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
                for (int i = 0; i < dicOrder.Count(); i++)
                {
                    if (i == dicOrder.Count() - 1)
                        requestStr.Append($"{dicOrder[i].Key}={dicOrder[i].Value}");
                    else
                        requestStr.Append($"{dicOrder[i].Key}={dicOrder[i].Value}&");
                }

                var utf8Request = SignCommon.GetUtf8(requestStr.ToString());

                var result = _verifySignCommon.GetSignhHash(utf8Request, _verifySignCommon.GetSignSecret(appIdString), encryptEnum);
                if (_verifySignOption.IsDebug)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    _logger.LogInformation($"方式：[VerifySign] 请求接口地址：{request.Request.Path}");
                    _logger.LogInformation($"方式：[VerifySign] 服务器签名前重新排序后的值{Convert.ToBase64String(Encoding.Default.GetBytes(utf8Request))}");
                    _logger.LogInformation($"方式：[VerifySign] 摘要比对： 服务器签名结果={result}   客户端签名结果={signvalue };equal={signvalue == result}");
                    _logger.LogInformation($"方式：[VerifySign] encryptEnum={encryptEnum}密钥={_verifySignCommon.GetSignSecret(appIdString)}");
                    Console.ResetColor();
                }
                else if (signvalue != result)
                {
                    _logger.LogWarning(@$"方式：[VerifySign] 摘要被篡改：[iphide]----{signvalue }
                                            查看详情，请设置VerifySignOption节点的IsDebug为true");

                    return Tuple.Create<bool, string>(false, "签名异常,请求非法");
                }

                return Tuple.Create<bool, string>(true, "签名通过");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "方式：[VerifySign] 签名异常");
                return Tuple.Create<bool, string>(false, "签名异常");
            }
        }

        private bool CheckTime(long requestTime, VerifySignOption _verifySignOption)
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
