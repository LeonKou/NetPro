using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace NetPro.ShareRequestBody
{
    internal class Common
    {
        internal static async Task<string> ReadAsString(HttpContext context)
        {
            try
            {
                if (context.Request.ContentLength > 0)
                {
                    EnableRewind(context.Request);
                    var encoding = GetRequestEncoding(context.Request);
                    return await ReadStreamRequest(context, encoding);
                }
                return null;

            }
            catch (Exception ex) when (ex.Message == "Unexpected end of request content.")
            {
                //_iLogger.LogError(ex, $"[ReadAsString] Post响应缓存出错,客户端取消请求");
                return null;
            }
        }

        internal static async Task<string> ReadStreamRequest(HttpContext context, Encoding encoding)
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

        internal static async Task<string> ReadStreamResponse(HttpContext context)
        {
            try
            {
                using (StreamReader sr = new StreamReader(context.Response.Body, Encoding.UTF8, true, 1024, true))
                {
                    if (context.RequestAborted.IsCancellationRequested)
                        return null;
                    var str = await sr.ReadToEndAsync();
                    context.Response.Body.Seek(0, SeekOrigin.Begin);
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
            }
            request.Body.Seek(0L, SeekOrigin.Begin);
        }
    }
}
