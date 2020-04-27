using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace NetPro.Web.Core.Helpers
{
    public class ActionFilterHelper
    {
        private static Encoding GetRequestEncoding(HttpRequest request)
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

        private static void EnableRewind(HttpRequest request)
        {
            if (!request.Body.CanSeek)
            {
                request.EnableBuffering();
                Task.WaitAll(request.Body.DrainAsync(CancellationToken.None));
            }
            request.Body.Seek(0L, SeekOrigin.Begin);
        }

        private static string ReadStream(Stream stream, Encoding encoding)
        {
            using (StreamReader sr = new StreamReader(stream, encoding, true, 1024, true))//这里注意Body部分不能随StreamReader一起释放
            {
                var str = sr.ReadToEnd();
                stream.Seek(0, SeekOrigin.Begin);//内容读取完成后需要将当前位置初始化，否则后面的InputFormatter会无法读取
                return str;
            }
        }

        private static string ReadAsString(HttpRequest request)
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
                //var initialBody = request.Body;
                //var buffer = new byte[Convert.ToInt32(request.ContentLength)];
                //long position = request.Body.Position;
                //request.Body.Position = 0;
                //request.Body.Read(buffer, 0, buffer.Length);
                //var json = Encoding.UTF8.GetString(buffer);
                //request.Body.Position = position;
                //request.Body = initialBody;
                //return json;

                //using (MemoryStream copyStream = new MemoryStream())
                //{
                //    if (request.Body.CanSeek)
                //        request.Body.Seek(0, SeekOrigin.Begin);
                //    request.Body.CopyTo(copyStream);
                //    byte[] buffer = new byte[(int)request.ContentLength];
                //    copyStream.Seek(0, SeekOrigin.Begin);
                //    copyStream.Read(buffer, 0, buffer.Length);
                //    var json = Encoding.UTF8.GetString(buffer);
                //    return json;
                //}
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        ///获取api请求body值
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static string GetRequestBodyText(HttpRequest request)
        {
            return GetRequestBodyText(request, null);
        }

        public static string GetRequestBodyText(HttpRequest request, string filterKeyRegex)
        {
            string requestBodyText = string.Empty;
            try
            {
                string method = request.Method.ToUpper();
                if (method == "POST" || method == "PUT" || method == "DELETE")
                {
                    if (request.HasFormContentType)
                    {
                        string content = FilterRequestKey(request.Form.ToDictionary(k => k.Key, v => HttpUtility.UrlEncode(v.Value.ToString())), filterKeyRegex);
                        requestBodyText = string.Concat(requestBodyText, content);
                    }
                    else
                    {
                        //request body 字符串+json 请求获取参数 
                        var content = ReadAsString(request);
                        requestBodyText = string.IsNullOrWhiteSpace(filterKeyRegex) ?
                            content : Regex.Replace(content, $"[ \\f\\r\\t\\v]*\\\"({filterKeyRegex})\\\":\\s[\\S \\f\\r\\t\\v]*\\n", "", RegexOptions.IgnoreCase);         //匹配JSON里面的KEY值对进行替换
                    }
                }
                else if (method == "GET")
                {
                    requestBodyText = FilterRequestKey(request.Query.ToDictionary(k => k.Key, v => HttpUtility.UrlEncode(v.Value.ToString())), filterKeyRegex);
                }
                return requestBodyText;
            }
            catch (Exception ex)
            {
                return requestBodyText;
            }
        }

        private static string FilterRequestKey(IDictionary<string, string> collection, string filterKeyRegex)
        {
            string result = string.Empty;
            if (collection != null && collection.Count > 0)
            {
                //表单提交获取请求值 AddParameter
                List<string> reqParams = new List<string>();
                foreach (string key in collection.Keys)
                {
                    if (!string.IsNullOrWhiteSpace(filterKeyRegex))
                    {
                        if (!Regex.IsMatch(key, filterKeyRegex, RegexOptions.IgnoreCase))
                        {
                            reqParams.Add(string.Format("{0}={1}", key, collection[key]));
                        }
                    }
                    else
                    {
                        reqParams.Add(string.Format("{0}={1}", key, collection[key]));
                    }
                }
                result = string.Join("&", reqParams);
            }
            return result;
        }
    }
}
