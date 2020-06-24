using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
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
        public VerifySignFilter(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var descriptor = (Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor)context.ActionDescriptor;
            var attribute = (IgnoreSignAttribute)descriptor.MethodInfo.GetCustomAttributes(typeof(IgnoreSignAttribute), true).FirstOrDefault();
            if (attribute != null)
                goto gotoNext;

            if (!GetSignValue(context.HttpContext.Request))
            {
                BuildErrorJson(context);
                await Task.CompletedTask;
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

                var timestampStr = queryDic["timestamp"];
                var now = (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
                if (!(long.TryParse(timestampStr, out long timestamp) && timestamp + 30 >= now))//原时间戳基础上减30秒与当前时间判断
                    return false;

                var appIdString = queryDic["appid"].ToString();
                if (string.IsNullOrEmpty(appIdString))
                {
                    Console.WriteLine(@"The request parameter is missing the Ak/Sk appID parameter
                                          VerifySign:{
                                            AppSecret:{
                                            [AppId]:[Secret]
                                                      }}");
                    return false;
                }

                var signvalue = queryDic["sign"].ToString();
                queryDic.Remove("sign");

                var bodyValue = ReadAsString(request);
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

                var result = GetSignhHash(requestStr.ToString(), GetGameSecret(appIdString));
                Console.WriteLine($"摘要计算后的值：{result}");

                Console.WriteLine($"{result}----{signvalue }");
                return signvalue == result;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 获取游戏secret
        /// </summary>
        /// <param name="gameId"></param>
        /// <returns></returns>
        private string GetGameSecret(string appid)
        {
            var secret = _configuration.GetValue<string>($"VerifySign:AppSecret:AppId:{appid}");

            return secret;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="secret"></param>
        /// <returns>签名16进制</returns>
        public string GetSignhHash(string message, string secret)
        {

            secret = secret ?? "";
            var encoding = new ASCIIEncoding();
            byte[] keyByte = encoding.GetBytes(secret);
            byte[] messageBytes = encoding.GetBytes(message);
            using (var hmacsha256 = new HMACSHA256(keyByte))
            {
                byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
                var hexString = hashmessage.Aggregate(new StringBuilder(),
                               (sb, v) => sb.Append(v.ToString("x2"))
                              ).ToString();
                return hexString;
            }
        }

        private string ReadAsString(HttpRequest request)
        {
            try
            {
                if (request.ContentLength > 0)
                {
                    EnableRewind(request);
                    var encoding = GetRequestEncoding(request);
                    return ReadStream(request.Body, encoding);
                }
                return null;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private string ReadStream(Stream stream, Encoding encoding)
        {
            using (StreamReader sr = new StreamReader(stream, encoding, true, 1024, true))
            {
                var str = sr.ReadToEnd();
                stream.Seek(0, SeekOrigin.Begin);
                return str;
            }
        }

        private Encoding GetRequestEncoding(HttpRequest request)
        {
            var requestContentType = request.ContentType;
            var requestMediaType = requestContentType == null ? default(MediaType) : new MediaType(requestContentType);
            var requestEncoding = requestMediaType.Encoding;
            if (requestEncoding == null)
            {
                requestEncoding = Encoding.UTF8;
            }
            return requestEncoding;
        }

        private void EnableRewind(HttpRequest request)
        {
            if (!request.Body.CanSeek)
            {
                request.EnableBuffering();
                Task.WaitAll(request.Body.DrainAsync(CancellationToken.None));
            }
            request.Body.Seek(0L, SeekOrigin.Begin);
        }

        private void BuildErrorJson(ActionExecutingContext context)
        {
            context.HttpContext.Response.StatusCode = 400;
            context.HttpContext.Response.ContentType = "application/json";
            context.Result = new BadRequestObjectResult(new { ErrorCode = -1, Message = $"签名验证失败" });
        }
    }
}
