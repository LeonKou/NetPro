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
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.ResponseCaching;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using NetPro.ShareRequestBody;

namespace NetPro.ResponseCache
{
    public class PostResponseCacheMiddleware
    {
        private readonly ILogger _iLogger;
        private readonly RequestDelegate _next;
        private IMemoryCache _memorycache;
        private ResponseCacheOption _responseCacheOption;

        private readonly IConfiguration _configuration;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="next"></param>
        /// <param name="configuration"></param>
        /// <param name="iLogger"></param>
        /// <param name="memorycache"></param>
        /// <param name="responseCacheOption"></param>
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
        /// <param name="responseCacheData">自定义对象不能ctor注入</param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext context, ResponseCacheData responseCacheData, RequestCacheData requestCacheData)
        {
            context.Request.EnableBuffering();
            var token = context.RequestAborted.Register(async () =>
            {
                _iLogger.LogWarning($"[PostResponse]请求被取消{context.Request.Path}");
                context.Response.StatusCode = 400;
                context.Response.ContentType = context.Request.ContentType;
                await Task.CompletedTask;
                return;
            });

            if (context.Request.Method.Equals("get", StringComparison.OrdinalIgnoreCase)
                || context.Request.Method.Equals("head", StringComparison.OrdinalIgnoreCase)
                || _memorycache.TryGetValue($"PostResponseCache_{context.Request.Path}", out object _tempIgnoe)
                || _memorycache.TryGetValue($"IgnorePostResponseCache_{context.Request.Path}", out object _temp))
            {
                await _next(context);
            }
            else
            {
                var convertedDictionatry = context.Request.Query.ToDictionary(s => s.Key.ToLower(), s => s.Value);

                foreach (var item in _responseCacheOption?.IgnoreVaryQuery ?? new List<string>())
                {
                    if (convertedDictionatry.ContainsKey(item.ToLower()))
                        convertedDictionatry.Remove(item.ToLower());
                }

                StringBuilder requestStrKey = new StringBuilder(context.Request.Path);
                foreach (var item in convertedDictionatry)
                {
                    requestStrKey.Append($"{item.Key}{item.Value}");
                }

                string bodyValue;
                if (requestCacheData == null || string.IsNullOrEmpty(requestCacheData.Body))
                {
                    bodyValue = await ReadAsString(context);
                    requestCacheData = new RequestCacheData { Body = bodyValue };
                }
                else
                    bodyValue = requestCacheData.Body;
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
                        _iLogger.LogInformation($"触发PostResponseCacheMiddleware本地缓存");
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
                                responseCacheData = new ResponseCacheData
                                {
                                    Body = responseBody,
                                    ContentType = context.Response.ContentType,
                                    StatusCode = context.Response.StatusCode
                                };
                                memStream.Position = 0;
                                await memStream.CopyToAsync(originalBody);

                                _memorycache.GetOrCreate<ResponseCacheData>($"NetProPostResponse:{requestStrKey}", s =>
                                {
                                    s.AbsoluteExpirationRelativeToNow = new TimeSpan(_responseCacheOption.Duration);
                                    return new ResponseCacheData
                                    {
                                        Body = responseBody,
                                        ContentType = context.Response.ContentType,
                                        StatusCode = context.Response.StatusCode
                                    };
                                });
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
            }
            request.Body.Seek(0L, SeekOrigin.Begin);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public static class PostResponseCacheMiddlewareExtensions
    {
        /// <summary>
        /// 签名在响应缓存之前
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>  
        /// <remarks>默认Get全局缓存</remarks>
        public static IApplicationBuilder UsePostResponseCache(
            this IApplicationBuilder builder)
        {
            var responseCacheOption = builder.ApplicationServices.GetService(typeof(ResponseCacheOption)) as ResponseCacheOption;           
            if (responseCacheOption.Enabled)
            {
                if (responseCacheOption.Duration < 1)
                    throw new ArgumentNullException($"ResponseCacheOption.Duration", "Post响应缓存Duration参数不能小于1");
                //脱离Http协议的Post缓存
                builder.UseMiddleware<PostResponseCacheMiddleware>();
            }

            return builder;
        }

        /// <summary>
        /// Get缓存
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>  
        /// <remarks>默认Get全局缓存</remarks>
        public static IApplicationBuilder UseGetResponseCaching(
            this IApplicationBuilder builder)
        {
            var responseCacheOption = builder.ApplicationServices.GetService(typeof(ResponseCacheOption)) as ResponseCacheOption;
            //全局Get响应缓存，遵守Http协议
            builder.UseResponseCaching();
            builder.Use(async (context, next) =>
            {
                context.Response.GetTypedHeaders().CacheControl =
                new Microsoft.Net.Http.Headers.CacheControlHeaderValue()
                {
                    Public = true,
                    MaxAge = TimeSpan.FromSeconds(responseCacheOption.Duration < 1 ? 1 : responseCacheOption.Duration)
                };

                var responseCachingFeature = context.Features.Get<IResponseCachingFeature>();

                if (responseCachingFeature != null)//必须放于响应缓存之后
                {
                    responseCachingFeature.VaryByQueryKeys = new[] { "*" };
                }
                await next();
            });

            return builder;
        }
    }
}
