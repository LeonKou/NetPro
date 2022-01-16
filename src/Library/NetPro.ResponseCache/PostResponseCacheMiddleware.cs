using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.ResponseCaching;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using NetPro.CsRedis;
using NetPro.ShareRequestBody;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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
                await Task.CompletedTask;
                return;
            });

            var endpoint = context.GetEndpoint();
            if (endpoint != null)
            {
                if (endpoint.Metadata
                .Any(m => m is IgnorePostResponseCacheAttribute))
                {
                    goto gotoNext;
                }
            }

            if (context.Request.Method.Equals("get", StringComparison.OrdinalIgnoreCase)
                || context.Request.Method.Equals("head", StringComparison.OrdinalIgnoreCase)
                || _memorycache.TryGetValue($"PostResponseCache_{context.Request.Path}", out object _tempIgnoe)
                || _memorycache.TryGetValue($"IgnorePostResponseCache_{context.Request.Path}", out object _temp))
            {
                goto gotoNext;
            }
            else
            {
                var convertedDictionatry = context.Request.Query.ToDictionary(s => s.Key.ToLower(), s => s.Value);

                foreach (var item in _responseCacheOption?.IgnoreVaryByQueryKeys ?? new List<string>())
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
                    bodyValue = await Common.ReadAsString(context);
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
                    ResponseCacheData cacheResponseBody = null;
                    IRedisManager _redisManager = null;
                    if (_responseCacheOption.Cluster)
                    {
                        _redisManager = context.RequestServices.GetService<IRedisManager>();
                        if (_redisManager == null)
                        {
                            throw new ArgumentNullException(nameof(RedisCacheOption), $"PostResponseCache组件在集群模式下未检测到注入NetPro.CsRedis组件,请检查是否遗漏{nameof(RedisCacheOption)}配置节点");
                        }
                        cacheResponseBody = _redisManager.Get<ResponseCacheData>($"NetProPostResponse:{requestStrKey}");
                    }
                    else
                    {
                        cacheResponseBody = _memorycache.Get<ResponseCacheData>($"NetProPostResponse:{requestStrKey}");
                    }

                    if (cacheResponseBody != null && !context.RequestAborted.IsCancellationRequested)
                    {
                        //https://stackoverflow.com/questions/45675102/asp-net-core-middleware-cannot-set-status-code-on-exception-because-response-ha
                        if (!context.Response.HasStarted)
                        {
                            context.Response.StatusCode = cacheResponseBody.StatusCode;
                            context.Response.ContentType = cacheResponseBody.ContentType;
                            context.Response.Headers["response-cache"] = "true";
                            await context.Response.WriteAsync(cacheResponseBody.Body);
                            _iLogger.LogInformation($"触发PostResponseCacheMiddleware本地缓存");
                            //直接return可避免此错误 ：OnStarting cannot be set because the response has already started.
                            await Task.CompletedTask;
                            return;
                        }
                        else
                        {
                            _iLogger.LogError($"StatusCode无法设置，因为响应已经启动,位置为:触发本地缓存开始赋值[responsecache2]");
                            await Task.CompletedTask;
                            return;
                        }
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
                                if (_responseCacheOption.Cluster)
                                {
                                    _redisManager.Set($"NetProPostResponse:{requestStrKey}", new ResponseCacheData
                                    {
                                        Body = responseBody,
                                        ContentType = context.Response.ContentType,
                                        StatusCode = context.Response.StatusCode
                                    }, TimeSpan.FromSeconds(_responseCacheOption.Duration));
                                }
                                else
                                {
                                    _memorycache.Set<ResponseCacheData>($"NetProPostResponse:{requestStrKey}", new ResponseCacheData
                                    {
                                        Body = responseBody,
                                        ContentType = context.Response.ContentType,
                                        StatusCode = context.Response.StatusCode
                                    }, TimeSpan.FromSeconds(_responseCacheOption.Duration));
                                }
                            }
                            await Task.CompletedTask;
                            return;
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
                    goto gotoNext;
                }
                else
                {
                    await Task.CompletedTask;
                    return;
                }
            }

        gotoNext:
            await _next(context);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public static class PostResponseCacheMiddlewareExtensions
    {
        // response cache not be suitable for global middleware
        /// <summary>
        /// 签名在响应缓存之前
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>  
        /// <remarks></remarks>
        //public static IApplicationBuilder UsePostResponseCache(
        //    this IApplicationBuilder builder)
        //{
        //    var responseCacheOption = builder.ApplicationServices.GetService(typeof(ResponseCacheOption)) as ResponseCacheOption;
        //    if (responseCacheOption?.Enabled ?? false)
        //    {
        //        if (responseCacheOption.Duration < 1)
        //            throw new ArgumentNullException($"ResponseCacheOption.Duration", "Post响应缓存Duration参数不能小于1");
        //        //脱离Http协议的Post缓存
        //        builder.UseMiddleware<PostResponseCacheMiddleware>();
        //    }

        //    return builder;
        //}

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
            if (responseCacheOption?.Enabled ?? false)
            {
                //全局Get响应缓存，遵守Http协议
                builder.UseResponseCaching();
                builder.Use(async (context, next) =>
                {
                    if (context.Request.Method.Equals("get", StringComparison.OrdinalIgnoreCase))
                    {
                        context.Response.GetTypedHeaders().CacheControl =
                     new CacheControlHeaderValue()
                     {
                         Public = true,
                         MaxAge = TimeSpan.FromSeconds(responseCacheOption.Duration < 1 ? 1 : responseCacheOption.Duration)
                     };

                        var responseCachingFeature = context.Features.Get<IResponseCachingFeature>();

                        if (responseCachingFeature != null)//必须放于响应缓存之后
                        {
                            responseCachingFeature.VaryByQueryKeys = new[] { "*" };
                        }

                    }
                    await next();
                });
            }
            return builder;
        }
    }
}
