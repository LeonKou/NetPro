using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Formatters;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NetPro.Sign
{
    /// <summary>
    /// 
    /// </summary>
    public class SignCommon
    {
        /// <summary>
        /// 通用生成签名
        /// </summary>
        /// <param name="secret"></param>
        /// <param name="query">url参数,参数名统一小写；参数中不能包含时间戳，时间戳已内部处理，参数名为：timestamp</param>
        /// <param name="body">body参数</param>
        /// <param name="signMethod">算法名称:hmac-sha256；md5</param>
        /// <remarks><![CDATA[ 将url参数与body参数以'&'分割]]>
        /// 拼装新字符串utf-8编码后
        /// HMACSHA256摘要后转16进制小写
        /// </remarks>
        /// <returns></returns>
        public static string CreateSign(string secret, NameValueCollection query, object body = null, EncryptEnum signMethod = EncryptEnum.Default, bool prinftLog = false)
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
            for (int i = 0; i < dicOrder.Count(); i++)
            {
                if (i == dicOrder.Count() - 1)
                    requestStr.Append($"{dicOrder[i].Key}={dicOrder[i].Value}");
                else
                    requestStr.Append($"{dicOrder[i].Key}={dicOrder[i].Value}&");
            }

            var utf8Request = GetUtf8(requestStr.ToString());

            string result;
            if (signMethod.HasFlag(EncryptEnum.Default) || signMethod.HasFlag(EncryptEnum.SignHMACSHA256))
            {
                result = GetHMACSHA256Sign(utf8Request, secret);
            }
            else if (signMethod.HasFlag(EncryptEnum.SignSHA256))
            {
                result = GetSHA256Sign(utf8Request, secret);
            }
            else if (signMethod.HasFlag(EncryptEnum.SignMD5))
            {
                result = CreateMD5(utf8Request, secret);
            }
            else
            {
                result = GetHMACSHA256Sign(utf8Request, secret);
            }

            if (prinftLog)
            {
                Console.WriteLine($"拼装排序后的值==>{utf8Request};摘要计算后的值==>{result}");
            }

            return result;
        }

        /// <summary>
        /// sha256
        /// </summary>
        /// <param name="message"></param>
        /// <param name="secret"></param>
        /// <returns></returns>
        internal static string GetSHA256Sign(string message, string secret)
        {
            byte[] data = Encoding.UTF8.GetBytes(message + secret);
            //SHA256 shaM = new SHA256Managed();
            using (SHA256 shaM = SHA256.Create())
            {
                var hashBytes = shaM.ComputeHash(data);
                var hexString = hashBytes.Aggregate(new StringBuilder(),
                                  (sb, v) => sb.Append(v.ToString("x2"))
                                 ).ToString();
                return hexString;
            }
        }
        internal static string GetHMACSHA256Sign(string message, string secret)
        {
            secret = secret ?? "";
            var encoding = new UTF8Encoding();
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
                byte[] hashBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(message + secret));

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
                    return await ReadStreamAsync(context.Request.Body, encoding);
                }
                return null;

            }
            catch (Exception ex) when (!ex.Message?.Replace(" ", string.Empty).ToLower().Contains("unexpectedendofrequestcontent") ?? true)
            {
                Console.WriteLine($"[ReadAsString] sign签名读取body出错");
                return null;
            }
        }

        private static async Task<string> ReadStreamAsync(Stream stream, Encoding encoding)
        {
            using (StreamReader sr = new StreamReader(stream, encoding, true, 1024, true))
            {
                var str = await sr.ReadToEndAsync();
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
                //try
                //{
                //    Task.WaitAll(request.Body.DrainAsync(CancellationToken.None)); //DrainAsync 导致内存飙升
                //}
                //catch (TaskCanceledException ex)
                //{
                //    Console.WriteLine($"[EnableRewind]Sign签名用户取消{request.Path}请求;exeptionMessage:{ex.Message}");
                //    return;
                //}

            }
            request.Body.Seek(0L, SeekOrigin.Begin);
        }

        /// <summary>
        /// 以json返回签名错误
        /// </summary>
        /// <param name="context"></param>
        /// <param name="msg"></param>
        internal static void BuildErrorJson(ActionExecutingContext context, string msg = "签名失败")
        {
            if (!context.HttpContext?.Response.HasStarted ?? false)
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
                context.HttpContext.Response.ContentType = "application/json";
            }

            context.Result = new BadRequestObjectResult(msg);
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestTime"></param>
        /// <param name="expireSeconds"></param>
        /// <returns></returns>
        internal static bool CheckTime(long requestTime, long expireSeconds)
        {
            if (requestTime < DateTimeOffset.Now.AddSeconds(-expireSeconds).ToUnixTimeSeconds())
            {
                return false;
            }

            long unixSeconds = DateTimeOffset.Now.ToUnixTimeSeconds();
            // 毫秒单位
            if (requestTime > 1265337794000)//毫秒1265337794000=2010-02-05 10:43:14
            {
                unixSeconds = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                expireSeconds = expireSeconds * 1000;
            }

            if (unixSeconds + expireSeconds < requestTime || requestTime < unixSeconds - expireSeconds)
            {
                return false;
            }
            return true;
        }
    }
}
