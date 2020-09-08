using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NetPro.ShareRequestBody;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NetPro.Sign
{
    public static class SignMiddlewareExtensions
    {
        public static IApplicationBuilder UseGlobalSign(
        this IApplicationBuilder builder)
        {
            var configuration = builder.ApplicationServices.GetService(typeof(IConfiguration)) as IConfiguration;

            if (configuration.GetValue<bool>("VerifySignOption:Enable", false))
                return builder.UseMiddleware<SignMiddleware>();
            return builder;
        }
    }

    public class SignMiddleware
    {
        private readonly ILogger _logger;
        private readonly RequestDelegate _next;
        private IMemoryCache _memorycache;
        private readonly IOperationFilter _verifySignCommon;

        private readonly IConfiguration _configuration;
        public SignMiddleware(RequestDelegate next, IConfiguration configuration,
            ILogger<SignMiddleware> iLogger,
            IMemoryCache memorycache,
            IOperationFilter verifySignCommon)
        {
            _next = next;
            _configuration = configuration;
            _logger = iLogger;
            _memorycache = memorycache;
            _verifySignCommon = verifySignCommon;
        }

        /// <summary>
        /// Post:（从头排序后+body json整体 ）hash
        /// </summary>
        /// <param name="context"></param>
        ///  <param name="responseCacheData"></param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext context, RequestCacheData requestCacheData, VerifySignOption verifySignOption)
        {
            if(!context.Request.Path.Value.StartsWith("/api"))
            {
                await _next(context);
                return;
            }
            context.Request.EnableBuffering();
            foreach (var item in verifySignOption.IgnoreRoute)
            {
                if (context.Request.Path.Value.Contains(item))
                {
                    _logger.LogInformation($"{context.Request.Path.Value}路径已绕过签名");
                    await _next(context);
                    return;
                }
            }

            var result = await GetSignValue(context, requestCacheData, verifySignOption);
            if (verifySignOption.IsForce && !result.Item1)
            {
                context.Response.StatusCode = 400;
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(new { Code = -1, Msg = $"{result.Item2}" }, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(System.Text.Unicode.UnicodeRanges.All)
                }));
                return;
            }
            else
                goto gotoNext;

            gotoNext:
            await _next(context);
        }

        private async Task<Tuple<bool, string>> GetSignValue(HttpContext request, RequestCacheData requestCacheData, VerifySignOption verifySignOption)
        {
            try
            {
                var convertedDictionatry = request.Request.Query.ToDictionary(s => s.Key, s => s.Value);
                var queryDic = new Dictionary<string, string>();
                foreach (var item in convertedDictionatry)
                {
                    queryDic.Add(item.Key.ToLower(), item.Value);
                }
                var commonParameters = verifySignOption.CommonParameters;
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
                if (!long.TryParse(timestampStr, out long timestamp) || !SignCommon.CheckTime(timestamp, verifySignOption.ExpireSeconds))
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
                StringBuilder logString = new StringBuilder();

                //格式  参数名参数值
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
                if (verifySignOption.IsDebug)
                {
                    _logger.LogInformation($"请求接口地址：{request.Request.Path}");
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
    }
}
