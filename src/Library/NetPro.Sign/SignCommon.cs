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

namespace NetPro.Sign
{
    public class SignCommon
    {
        /// <summary>
        /// 生成签名
        /// </summary>
        /// <param name="secret"></param>
        /// <param name="queryDic">url参数,参数名统一小写；参数中不能包含时间戳，时间戳已内部处理，参数名为：timestamp</param>
        /// <param name="body">body参数</param>
        /// <remarks>将url参数与body参数以&分割
        /// 拼装新字符串utf-8编码后
        /// HMACSHA256摘要后转16进制小写
        /// </remarks>
        /// <returns></returns>
        public static string CreateSign(string secret, Dictionary<string, string> queryDic, object body = null)
        {
            if (queryDic == null || !queryDic.Any())
            {
                Console.WriteLine("签名公共参数必须以url方式提供");
                return string.Empty;
            }

            if (body != null)
            {
                var jsonString = JsonSerializer.Serialize(body);
                var dict = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonString);
                foreach (var item in dict)
                {
                    queryDic.Add(item.Key, item.Value.ToString());
                    Console.WriteLine($"字段:{item.Key}--值:{item.Value}");
                }
            }

            var dicOrder = queryDic.OrderBy(s => s.Key, StringComparer.Ordinal).ToList();

            StringBuilder requestStr = new StringBuilder();
            //requestStr.Append(string.Join("&", dicOrder.Select(a => $"{a.Key}={a.Value?.ToString().Trim()}")));

            for (int i = 0; i < dicOrder.Count(); i++)
            {
                if (i == dicOrder.Count() - 1)
                    requestStr.Append($"{dicOrder[i].Key}={dicOrder[i].Value}");
                else
                    requestStr.Append($"{dicOrder[i].Key}={dicOrder[i].Value}&");
            }

            var utf8Request = GetUtf8(requestStr.ToString());

            var result = GetSignhHash(utf8Request, secret);
            Console.WriteLine($"拼装排序后的值==>{requestStr};摘要计算后的值==>{result}");
            return result;
        }

        internal static string GetSignhHash(string message, string secret)
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

        internal static string GetUtf8(string unicodeString)
        {
            UTF8Encoding utf8 = new UTF8Encoding();
            Byte[] encodedBytes = utf8.GetBytes(unicodeString);
            String decodedString = utf8.GetString(encodedBytes);
            return decodedString;
        }

        internal static string ReadAsString(HttpRequest request)
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
                Console.WriteLine($"读取requet的body出错：{ex}");
                return null;
            }
        }

        internal static string ReadStream(Stream stream, Encoding encoding)
        {
            using (StreamReader sr = new StreamReader(stream, encoding, true, 1024, true))
            {
                var str = sr.ReadToEnd();
                stream.Seek(0, SeekOrigin.Begin);
                return str;
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
        internal static void BuildErrorJson(ActionExecutingContext context)
        {
            context.HttpContext.Response.StatusCode = 400;
            context.HttpContext.Response.ContentType = "application/json";
            context.Result = new BadRequestObjectResult(new { ErrorCode = -1, Message = $"签名验证失败" });
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
    }
}
