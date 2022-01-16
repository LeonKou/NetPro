using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NetPro.ShareRequestBody;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NetPro.Sign
{
    public static class SignMiddlewareExtensions
    {
        /// <summary>
        /// 全局请求接口签名，只校验/api开头路由
        /// 必须在UseRouting之后
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseGlobalSign(
        this IApplicationBuilder builder)
        {
            var verifySignOption = builder.ApplicationServices.GetService<VerifySignOption>();//(typeof(IConfiguration)) as IConfiguration;

            //if (configuration.GetValue<bool>("VerifySignOption:Disabled", false))
            if (verifySignOption?.Enabled ?? false)
                return builder.UseMiddleware<SignMiddleware>();
            return builder;
        }
    }

    public class SignMiddleware
    {
        private readonly ILogger _logger;
        private readonly RequestDelegate _next;
        private IMemoryCache _memorycache;

        private readonly IConfiguration _configuration;
        public SignMiddleware(RequestDelegate next, IConfiguration configuration,
            ILogger<SignMiddleware> iLogger,
            IMemoryCache memorycache)
        {
            _next = next;
            _configuration = configuration;
            _logger = iLogger;
            _memorycache = memorycache;
        }

        /// <summary>
        /// Post:（从头排序后+body json整体 ）hash
        /// </summary>
        /// <param name="context"></param>
        ///  <param name="requestCacheData"></param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext context, RequestCacheData requestCacheData, VerifySignOption verifySignOption, IOperationFilter _verifySignCommon)
        {
            if (!context.Request.Path.Value.ToLower().StartsWith("/api"))
            {
                await _next(context);
                return;
            }
            var endpoint = context.Features.Get<IEndpointFeature>()?.Endpoint;
            if (endpoint != null)
            {
                if (endpoint.Metadata
                .Any(m => m is IgnoreSignAttribute))
                {
                    _logger.LogInformation($"{context.Request.Path.Value}路径已绕过签名;如设置了必须签名依旧没命中,请检查HTTP method是否匹配;Method={context.Request.Method}");
                    await _next(context);
                    return;
                }
            }

            context.Request.EnableBuffering();

            var result = await GetSignValue(context, requestCacheData, verifySignOption, _verifySignCommon);
            if (verifySignOption.IsForce && !result.Item1)
            {
                if (!context?.Response.HasStarted ?? false)
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    context.Response.ContentType = "application/text";
                }

                await context.Response.WriteAsync(result.Item2);
                await Task.CompletedTask;
                return;
            }
            else
                goto gotoNext;

            gotoNext:
            await _next(context);
        }

        private async Task<Tuple<bool, string>> GetSignValue(HttpContext request, RequestCacheData requestCacheData, VerifySignOption verifySignOption, IOperationFilter _verifySignCommon)
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

                var commonParameters = verifySignOption.CommonParameters;

                if (!queryDic.ContainsKey(commonParameters.TimestampName) || !queryDic.ContainsKey(commonParameters.AppIdName) || !queryDic.ContainsKey(commonParameters.SignName))
                {
                    _logger.LogWarning("方式：[Middleware] url参数中未找到签名所需参数[timestamp];[appid];[EncryptFlag]或[sign]");
                    return Tuple.Create<bool, string>(false, "签名参数缺失");
                }

                if (queryDic.ContainsKey(commonParameters.EncryptFlag) && int.TryParse(queryDic[commonParameters.EncryptFlag].ToString(), out int encryptint))
                {
                    encryptEnum = (EncryptEnum)encryptint;
                }
                //认证token也参与签名
                if (!string.IsNullOrWhiteSpace(request.Request.Headers["Authorization"]))
                {
                    queryDic.Add("authorization", request.Request.Headers["Authorization"]);
                }
                //语言信息参与签名
                if (!string.IsNullOrWhiteSpace(request.Request.Headers["Accept-Language"]))
                {
                    queryDic.Add("accept-language", request.Request.Headers["Accept-Language"]);
                }

                var timestampStr = queryDic[commonParameters.TimestampName];
                if (!long.TryParse(timestampStr, out long timestamp) || !SignCommon.CheckTime(timestamp, verifySignOption.ExpireSeconds))
                {
                    _logger.LogWarning($"方式：[Middleware] {timestampStr}时间戳已过期");
                    return Tuple.Create<bool, string>(false, "请校准客户端时间后再试");
                }

                var appIdString = queryDic[commonParameters.AppIdName].ToString();
                if (string.IsNullOrEmpty(appIdString))
                {
                    _logger.LogWarning(@"方式：[Middleware] The request parameter is missing the Ak/Sk appID parameter
                                          VerifySign:{
                                            AppSecret:{
                                            [AppId]:[Secret]
                                                      }}");
                    return Tuple.Create<bool, string>(false, "服务异常，AppIdName未配置");
                }

                var signvalue = queryDic[commonParameters.SignName].ToString();
                queryDic.Remove(commonParameters.SignName);

                string bodyValue;
                if (requestCacheData == null || string.IsNullOrEmpty(requestCacheData.Body))
                {
                    bodyValue = await SignCommon.ReadAsStringAsync(request);
                    requestCacheData = new RequestCacheData { Body = bodyValue };
                }
                else
                    bodyValue = requestCacheData.Body;

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
                if (verifySignOption.IsDebug)
                {
                    _logger.LogInformation($"方式：[Middleware] 请求接口地址：{request.Request.Path}");
                    _logger.LogInformation($"方式：[Middleware] 服务器签名前重新排序后的值={Convert.ToBase64String(Encoding.Default.GetBytes(utf8Request))}");
                    _logger.LogInformation($"方式：[Middleware] 摘要比对： 服务器签名结果={result}   客户端签名结果={signvalue };equal={signvalue == result}");
                    _logger.LogInformation($"方式：[Middleware] encryptEnum={encryptEnum}密钥={_verifySignCommon.GetSignSecret(appIdString)}");
                }
                else if (signvalue != result)
                {
                    _logger.LogWarning(@$"方式：[Middleware] 摘要被篡改：[iphide]----{signvalue }
                                            查看详情，请设置VerifySignOption节点的IsDebug为true");
                    return Tuple.Create<bool, string>(false, "签名异常,请求非法");
                }
                return Tuple.Create<bool, string>(true, "签名通过");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "方式：[Middleware] 签名异常");
                return Tuple.Create<bool, string>(false, "签名异常");
            }
        }
    }
}
