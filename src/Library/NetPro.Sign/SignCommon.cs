using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;
using System.Linq;
using System.Security.Cryptography;
using Microsoft.VisualBasic.CompilerServices;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using System.Text.Encodings.Web;

namespace NetPro.Sign
{
    public class SignCommon
    {
        /// <summary>
        /// 通用生成签名
        /// </summary>
        /// <param name="secret"></param>
        /// <param name="queryDic">url参数,参数名统一小写；参数中不能包含时间戳，时间戳已内部处理，参数名为：timestamp</param>
        /// <param name="body">body参数</param>
        /// <param name="signMethod">算法名称:hmac-sha256；md5</param>
        /// <remarks>将url参数与body参数以&分割
        /// 拼装新字符串utf-8编码后
        /// HMACSHA256摘要后转16进制小写
        /// </remarks>
        /// <returns></returns>
        public static string CreateSign(string secret, NameValueCollection query, object body = null, string signMethod = "")
        {
            IDictionary<string, string> queryDic = new Dictionary<string, string>();
            foreach (var k in query.AllKeys)
            {
                queryDic.Add(k, query[k]);
            }

            if (queryDic == null || !queryDic.Any())
            {
                Console.WriteLine("签名公共参数必须以url方式提供");
                return string.Empty;
            }

            if (body != null)
            {
                var jsonString = JsonSerializer.Serialize(body, new JsonSerializerOptions { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping });
                queryDic.Add("body", Regex.Replace(jsonString, @"\s(?=([^""]*""[^""]*"")*[^""]*$)", string.Empty));

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

            var utf8Request = GetUtf8(requestStr.ToString());

            string result;
            switch (signMethod)
            {
                case "hmac-sha256":
                    result = GetHMACSHA256Sign(utf8Request, secret);
                    break;
                case "md5":
                    result = CreateMD5(utf8Request, secret);
                    break;
                default:
                    result = GetHMACSHA256Sign(utf8Request, secret);
                    break;
            }

            Console.WriteLine($"拼装排序后的值==>{logString};摘要计算后的值==>{result}");
            return result;
        }

        internal static string GetHMACSHA256Sign(string message, string secret)
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

        /// <summary>
        /// MD5获取签名
        /// </summary>
        /// <param name="message"></param>
        /// <param name="secret"></param>
        /// <returns></returns>
        internal static string CreateMD5(string message, string secret)
        {
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] hashBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(secret + message + secret));

                var hexString = hashBytes.Aggregate(new StringBuilder(),
                               (sb, v) => sb.Append(v.ToString("x2"))
                              ).ToString();
                return hexString;
            }
        }

        internal static string GetUtf8(string unicodeString)
        {
            UTF8Encoding utf8 = new UTF8Encoding();
            Byte[] encodedBytes = utf8.GetBytes(unicodeString);
            String decodedString = utf8.GetString(encodedBytes);
            return decodedString;
        }

        internal static async Task<string> ReadAsStringAsync(HttpContext context)
        {
            try
            {
                if (context.Request.ContentLength > 0)
                {
                    EnableRewind(context.Request);
                    var encoding = GetRequestEncoding(context.Request);
                    return await ReadStream(context, encoding);
                }
                return null;

            }
            catch (Exception ex) when (ex.Message == "Unexpected end of request content.")
            {
                //_iLogger.LogError(ex, $"[ReadAsString] Post响应缓存出错,客户端取消请求");
                return null;
            }
        }

        internal static async Task<string> ReadStream(HttpContext context, Encoding encoding)
        {
            try
            {
                using (StreamReader sr = new StreamReader(context.Request.Body, encoding, true, 1024, true))
                {
                    if (context.RequestAborted.IsCancellationRequested)
                        return null;
                    var str = await sr.ReadToEndAsync();
                    context.Request.Body.Seek(0, SeekOrigin.Begin);
                    return str;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        internal static Encoding GetRequestEncoding(HttpRequest request)
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

        internal static void EnableRewind(HttpRequest request)
        {
            if (!request.Body.CanSeek)
            {
                request.EnableBuffering();
                Task.WaitAll(request.Body.DrainAsync(CancellationToken.None));
            }
            request.Body.Seek(0L, SeekOrigin.Begin);
        }

        /// <summary>
        /// 以json返回签名错误
        /// </summary>
        /// <param name="context"></param>
        internal static void BuildErrorJson(ActionExecutingContext context,string msg="签名失败")
        {
            context.HttpContext.Response.StatusCode = 400;
            context.HttpContext.Response.ContentType = "application/json";
            context.Result = new BadRequestObjectResult(new { Code = -1, Msg = msg });
        }

        /// <summary>
        /// 生成当前时间戳
        /// </summary>
        /// <returns></returns>
        public static string CreateTimestamp()
        {
            long unixSeconds = DateTimeOffset.Now.ToUnixTimeSeconds();
            return unixSeconds.ToString();
        }

        /// <summary>
        /// 生成指定时间戳
        /// </summary>
        /// <returns></returns>
        public static string CreateTimestamp(DateTime time)
        {
            long unixSeconds = new DateTimeOffset(time).ToUnixTimeSeconds();
            return unixSeconds.ToString();
        }


        internal static bool CheckTime(long requestTime, long expireSeconds)
        {
            long unixSeconds = DateTimeOffset.Now.ToUnixTimeSeconds();

            if (requestTime + expireSeconds - unixSeconds < 0)
            {
                return false;
            }
            return true;
        }
    }
}
