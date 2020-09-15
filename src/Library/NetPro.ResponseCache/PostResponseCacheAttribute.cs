using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NetPro.ResponseCache;
using NetPro.ShareRequestBody;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NetPro.Web.Core.Filters
{
    /// <summary>
    /// Post响应缓存
    /// 优先级高于UsePostResponseCache全局Post缓存
    /// </summary>
    /// <remarks>特性方式继承自动生效
    /// Order越小先执行</remarks>
    public class PostResponseCacheAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// 缓存持续时长
        /// </summary>
        public int Duration { get; set; }

        ///// <summary>
        ///// 
        ///// </summary>
        //public string VaryByHeader { get; set; }

        /// <summary>
        /// 忽略变化的query参数
        /// </summary>
        public string[] IgnoreVaryByQueryKeys { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            context.HttpContext.Request.EnableBuffering();
            if (Duration == 0)
                goto gotoNext;

            IServiceProvider serviceProvider = context.HttpContext.RequestServices;
            var _logger = serviceProvider.GetRequiredService<ILogger<PostResponseCacheAttribute>>();
            var _configuration = serviceProvider.GetRequiredService<IConfiguration>();
            var _memorycache = serviceProvider.GetService<IMemoryCache>();
            if (_memorycache == null)
            {
                throw new ArgumentNullException(nameof(IMemoryCache), "Post响应缓存依赖IMemoryCache，请services.AddMemoryCache()注入IMemoryCache后再使用[PostResponseCache]");
            }
            _memorycache.Set($"PostResponseCache_{context.HttpContext.Request.Path}", true);

            var _requestCacheData = serviceProvider.GetService<RequestCacheData>();
            if (_requestCacheData == null)
            {
                throw new ArgumentNullException(nameof(IMemoryCache), "Post响应缓存依赖ResponseCacheData，请调用services.AddShareRequestBody()注入后再使用[PostResponseCache]");
            }
            var _responseCacheOption = serviceProvider.GetService<ResponseCacheOption>();

            context.HttpContext.Request.EnableBuffering();
            var token = context.HttpContext.RequestAborted.Register(async () =>
            {
                _logger.LogWarning($"[PostResponse]请求被取消{context.HttpContext.Request.Path}");
                context.HttpContext.Response.StatusCode = 400;
                context.HttpContext.Response.ContentType = context.HttpContext.Request.ContentType;
                await Task.CompletedTask;
                return;
            });

            var descriptor = (Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor)context.ActionDescriptor;

            var attribute = (IgnorePostResponseCacheAttribute)descriptor.MethodInfo.GetCustomAttributes(typeof(IgnorePostResponseCacheAttribute), true).FirstOrDefault();
            if (attribute != null)
            {
                _memorycache.Set($"IgnorePostResponseCache_{context.HttpContext.Request.Path}", true);
                goto gotoNext;
            }

            if (context.HttpContext.Request.Method.Equals("get", StringComparison.OrdinalIgnoreCase) || context.HttpContext.Request.Method.Equals("head", StringComparison.OrdinalIgnoreCase))
            {
                goto gotoNext;
            }
            else
            {
                var convertedDictionatry = context.HttpContext.Request.Query.ToDictionary(s => s.Key.ToLower(), s => s.Value);

                foreach (var item in IgnoreVaryByQueryKeys ?? new string[0])
                {
                    if (convertedDictionatry.ContainsKey(item.ToLower()))
                        convertedDictionatry.Remove(item.ToLower());
                }

                StringBuilder requestStrKey = new StringBuilder(context.HttpContext.Request.Path);
                foreach (var item in convertedDictionatry)
                {
                    requestStrKey.Append($"{item.Key}{item.Value}");
                }

                string bodyValue;
                if (_requestCacheData == null || string.IsNullOrEmpty(_requestCacheData.Body))
                {
                    bodyValue = await ReadAsString(context.HttpContext);
                    _requestCacheData = new RequestCacheData { Body = bodyValue };
                }
                else
                    bodyValue = _requestCacheData.Body;
                if (!string.IsNullOrEmpty(bodyValue) && !"null".Equals(bodyValue))
                {
                    //非Get请求body有值才被缓存，其他默认不缓存，防止body读取失败导致缓存异常
                    bodyValue = Regex.Replace(bodyValue, @"\s(?=([^""]*""[^""]*"")*[^""]*$)", string.Empty);
                    bodyValue = bodyValue.Replace("\r\n", "").Replace(" : ", ":").Replace("\n  ", "").Replace("\n", "").Replace(": ", ":").Replace(", ", ",");

                    requestStrKey.Append($"body{bodyValue}");

                    var cacheResponseBody = _memorycache.Get<ResponseCacheData>($"NetProPostResponse:{requestStrKey}");
                    if (cacheResponseBody != null && !context.HttpContext.RequestAborted.IsCancellationRequested)
                    {
                        context.HttpContext.Response.StatusCode = cacheResponseBody.StatusCode;
                        context.HttpContext.Response.ContentType = cacheResponseBody.ContentType;
                        await context.HttpContext.Response.WriteAsync(cacheResponseBody.Body);
                        _logger.LogInformation($"触发PostResponseCacheAttribute本地缓存");
                        await Task.CompletedTask;
                        return;
                    }
                    else if (!context.HttpContext.RequestAborted.IsCancellationRequested)
                    {
                        try
                        {
                            var actionResult = await next();
                            dynamic responseResult = (dynamic)actionResult.Result;
                            if (actionResult.Exception != null)
                            {
                                // 过滤器中await next();执行的始终是Action而不是下一个过滤器或者中间件
                                await Task.CompletedTask;
                                return;
                            }
                            if (responseResult == null)
                            {
                                await Task.CompletedTask;
                                return;
                            }
                            var body = JsonSerializer.Serialize(responseResult.Value, new JsonSerializerOptions
                            {
                                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(System.Text.Unicode.UnicodeRanges.All)
                            });
                            _memorycache.Set($"NetProPostResponse:{requestStrKey}", new ResponseCacheData
                            {
                                Body = body,
                                ContentType = "application/json",
                                StatusCode = responseResult.StatusCode
                            }, TimeSpan.FromSeconds(Duration));
                        }
                        catch (Exception ex)
                        {
                            await Task.CompletedTask;
                            return;
                        }
                        await Task.CompletedTask;
                        return;
                    }
                    else if (context.HttpContext.RequestAborted.IsCancellationRequested)
                    {
                        await Task.CompletedTask;
                        return;
                    }
                }
                else if (!context.HttpContext.RequestAborted.IsCancellationRequested)
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
            await next();
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
}
