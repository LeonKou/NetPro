using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
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
        private readonly IConfiguration _configuration;
        private readonly IOperationFilter _verifySignCommon;
        private readonly VerifySignOption _verifySignOption;

        public VerifySignFilter(IConfiguration configuration, IOperationFilter verifySignCommon, VerifySignOption verifySignOption)
        {
            _configuration = configuration;
            _verifySignCommon = verifySignCommon;
            _verifySignOption = verifySignOption;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var descriptor = (Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor)context.ActionDescriptor;
            var attribute = (IgnoreSignAttribute)descriptor.MethodInfo.GetCustomAttributes(typeof(IgnoreSignAttribute), true).FirstOrDefault();
            if (attribute != null)
                goto gotoNext;

            if (!GetSignValue(context.HttpContext.Request))
            {
                new VerifySignCommon(_configuration).BuildErrorJson(context);
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
                    Console.WriteLine("url参数中未找到签名所需参数[timestamp];[appid]或[sign]");
                    return false;
                }

                var timestampStr = queryDic[commonParameters.TimestampName];
                if (!long.TryParse(timestampStr, out long timestamp) || !CheckTime(timestamp))//原时间戳基础上减30秒与当前时间判断
                    return false;

                var appIdString = queryDic[commonParameters.AppIdName].ToString();
                if (string.IsNullOrEmpty(appIdString))
                {
                    Console.WriteLine(@"The request parameter is missing the Ak/Sk appID parameter
                                          VerifySign:{
                                            AppSecret:{
                                            [AppId]:[Secret]
                                                      }}");
                    return false;
                }

                var signvalue = queryDic[commonParameters.SignName].ToString();
                queryDic.Remove(commonParameters.SignName);

                var bodyValue = new VerifySignCommon(_configuration).ReadAsString(request);
                if (!string.IsNullOrEmpty(bodyValue) && !"null".Equals(bodyValue))
                {
                    var dict = JsonSerializer.Deserialize<Dictionary<string, string>>(bodyValue);
                    foreach (var item in dict)
                    {
                        queryDic.Add(item.Key, item.Value);
                        Console.WriteLine($"字段:{item.Key}--值:{item.Value}");
                    }
                    queryDic = queryDic.OrderBy(s => s.Key, StringComparer.Ordinal).ToDictionary(s => s.Key, s => s.Value);
                }

                var dicOrder = queryDic.OrderBy(s => s.Key).ToList();

                StringBuilder requestStr = new StringBuilder();
                for (int i = 0; i < dicOrder.Count(); i++)
                {
                    if (i == dicOrder.Count() - 1)
                        requestStr.Append($"{dicOrder[i].Key}={dicOrder[i].Value}");
                    else
                        requestStr.Append($"{dicOrder[i].Key}={dicOrder[i].Value}&");
                }
                Console.WriteLine($"拼装排序后的值{requestStr}");

                var utf8Request = GetUtf8(requestStr.ToString());

                var result = _verifySignCommon.GetSignhHash(utf8Request, _verifySignCommon.GetSignSecret(appIdString));
                Console.WriteLine($"摘要计算后的值：{result}");

                Console.WriteLine($"{result}----{signvalue }");
                return signvalue == result;
            }
            catch
            {
                throw;
            }
        }

        private static string GetUtf8(string unicodeString)
        {
            UTF8Encoding utf8 = new UTF8Encoding();
            Byte[] encodedBytes = utf8.GetBytes(unicodeString);
            String decodedString = utf8.GetString(encodedBytes);
            return decodedString;
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
