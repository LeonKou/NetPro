using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;

namespace NetPro.ResponseCache
{
    public class PostResponseCacheMiddleware
    {
        private readonly ILogger _iLogger;
        private readonly RequestDelegate _next;
        private IMemoryCache _memorycache;
        private ResponseCacheOption _responseCacheOption;

        private readonly IConfiguration _configuration;
        public PostResponseCacheMiddleware(RequestDelegate next, IConfiguration configuration,
            ILogger<PostResponseCacheMiddleware> iLogger,
            IMemoryCache memorycache,
            ResponseCacheOption responseCacheOption)
        {
            _next = next;
            _configuration = configuration;
            _iLogger = iLogger;
            _memorycache = memorycache;
            _responseCacheOption = responseCacheOption;
        }

        /// <summary>
        /// Post:（从头排序后+body json整体 ）hash
        /// </summary>
        /// <param name="context"></param>
        ///  <param name="responseCacheData"></param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext context, ResponseCacheData responseCacheData)
        {
            var token = context.RequestAborted.Register(async () =>
            {
                _iLogger.LogWarning($"[PostResponse]请求被取消{context.Request.Path}");
                context.Response.StatusCode = 400;
                context.Response.ContentType = context.Request.ContentType;
                await Task.CompletedTask;
                return;
            });

            if (context.Request.Method.Equals("get", StringComparison.OrdinalIgnoreCase) || context.Request.Method.Equals("head", StringComparison.OrdinalIgnoreCase))
            {
                await _next(context);
            }
            else
            {
                var convertedDictionatry = context.Request.Query.ToDictionary(s => s.Key, s => s.Value);
                var queryDic = new Dictionary<string, string>();
                foreach (var item in convertedDictionatry)
                {
                    queryDic.Add(item.Key.ToLower(), item.Value);
                }
                foreach (var item in _responseCacheOption?.ExcluedQuery ?? new List<string>())
                {
                    if (queryDic.ContainsKey(item))
                        queryDic.Remove(item);
                }

                StringBuilder requestStrKey = new StringBuilder();
                foreach (var item in queryDic)
                {
                    requestStrKey.Append($"{item.Key}{item.Value}");
                }

                string bodyValue;
                if (responseCacheData == null || string.IsNullOrEmpty(responseCacheData.Body))
                {
                    bodyValue = await ReadAsString(context);
                    responseCacheData.Body = bodyValue;
                }
                else
                    bodyValue = responseCacheData.Body;
                if (!string.IsNullOrEmpty(bodyValue) && !"null".Equals(bodyValue))
                {
                    //非Get请求body有值才被缓存，其他默认不缓存，防止body读取失败导致缓存异常
                    bodyValue = Regex.Replace(bodyValue, @"\s(?=([^""]*""[^""]*"")*[^""]*$)", string.Empty);
                    bodyValue = bodyValue.Replace("\r\n", "").Replace(" : ", ":").Replace("\n  ", "").Replace("\n", "").Replace(": ", ":").Replace(", ", ",");

                    requestStrKey.Append($"body{bodyValue}");

                    var cacheResponseBody = _memorycache.Get<ResponseCacheData>($"NetProPostResponse:{requestStrKey}");
                    if (cacheResponseBody != null && !context.RequestAborted.IsCancellationRequested)
                    {
                        context.Response.StatusCode = cacheResponseBody.StatusCode;
                        context.Response.ContentType = cacheResponseBody.ContentType;
                        await context.Response.WriteAsync(cacheResponseBody.Body);
                    }
                    else if (!context.RequestAborted.IsCancellationRequested)
                    {
                        Stream originalBody = context.Response.Body;
                        try
                        {
                            using (var memStream = new MemoryStream())
                            {
                                context.Response.Body = memStream;

                                await _next(context);

                                memStream.Position = 0;
                                string responseBody = new StreamReader(memStream).ReadToEnd();

                                memStream.Position = 0;
                                await memStream.CopyToAsync(originalBody);
                                var now = DateTime.Now;
                                var expiredTimespan = now.AddSeconds(_responseCacheOption.Expired) - now;

                                _memorycache.Set<ResponseCacheData>($"NetProPostResponse:{requestStrKey}", new ResponseCacheData
                                {
                                    Body = responseBody,
                                    ContentType = context.Response.ContentType,
                                    StatusCode = context.Response.StatusCode
                                }, expiredTimespan);
                            }
                        }
                        finally
                        {
                            context.Response.Body = originalBody;
                        }
                    }
                    else if (context.RequestAborted.IsCancellationRequested)
                    {
                        await Task.CompletedTask;
                        return;
                    }
                }
                else if (!context.RequestAborted.IsCancellationRequested)
                {
                    await _next(context);
                }
                else
                {
                    await Task.CompletedTask;
                    return;
                }
            }
        }

        private async Task<string> ReadAsString(HttpContext context)
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

        private async Task<string> ReadStream(HttpContext context, Encoding encoding)
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
                //try
                //{
                //    //new CancellationToken();
                //    //Task.WaitAll(request.Body.DrainAsync(CancellationToken.None));
                //}
                //catch (TaskCanceledException ex)
                //{
                //    _iLogger.LogWarning(ex, $"[EnableRewind]post响应缓存用户取消{request.Path}请求;exeptionMessage:{ex.Message}");
                //    return;
                //}

            }
            request.Body.Seek(0L, SeekOrigin.Begin);
        }
    }

    public static class PostResponseCacheMiddlewareExtensions
    {
        public static IApplicationBuilder UsePostResponseCache(
            this IApplicationBuilder builder)
        {
            //var configuration = builder.ApplicationServices.GetService(typeof(IConfiguration)) as IConfiguration;

            var responseCacheOption = builder.ApplicationServices.GetService(typeof(ResponseCacheOption)) as ResponseCacheOption;

            if (responseCacheOption.Enabled)
                return builder.UseMiddleware<PostResponseCacheMiddleware>();
            return builder;
        }
    }

    public class ResponseCacheData
    {
        public string Body { get; set; }

        public int StatusCode { get; set; }

        public string ContentType { get; set; }
    }
}
