using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NetPro.CsRedis;
using NetPro.ShareRequestBody;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NetPro.ResponseCache
{
    public enum ResponseMode
    {
        /// <summary>
        /// 缓存,默认
        /// </summary>
        Cache = 0,

        /// <summary>
        /// 报错,触发响应缓存返回400状态码。可用于请求频率的限制，例如一秒中只能请求一次
        /// </summary>
        Error = 1,
    }

    /// <summary>
    /// Post响应缓存
    /// 优先级高于UsePostResponseCache全局Post缓存
    /// </summary>
    /// <remarks>特性方式继承自动生效
    /// Order越小先执行;attribute方式在中间件之后运行</remarks>
    public class PostResponseCacheAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// 是否集群模式，启用分布式Redis功能
        /// </summary>
        public bool Cluster { get; set; }

        public ResponseMode ResponseMode { get; set; } = ResponseMode.Cache;

        /// <summary>
        /// 配合ResponseMode.Error模式使用，当触发缓存时的提示信息
        /// </summary>
        public string Message { get; set; } = "The request has been cached";

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
            IServiceProvider serviceProvider = context.HttpContext.RequestServices;
            var responseCacheOption = serviceProvider.GetService<ResponseCacheOption>();

            //配置存在且配置关闭或者持续时长等于0，即忽略响应缓存
            if (responseCacheOption == null || !responseCacheOption.Enabled || Duration == 0)
            {
                goto gotoNext;
            }
            //context.HttpContext.Request.EnableBuffering();
            var _logger = serviceProvider.GetRequiredService<ILogger<PostResponseCacheAttribute>>();
            var _configuration = serviceProvider.GetRequiredService<IConfiguration>();
            var _memorycache = serviceProvider.GetService<IMemoryCache>();
            IRedisManager _redisManager = null;
            if ((responseCacheOption != null && responseCacheOption.Cluster) || Cluster)
            {
                _redisManager = serviceProvider.GetService<IRedisManager>();
                if (_redisManager == null)
                {
                    throw new ArgumentNullException(nameof(IMemoryCache), "Post响应已启用集群模式，缓存依赖IRedisManager，请services.AddRedisManager()注入IRedisManager后再使用[PostResponseCache]");
                }
            }
            if (_memorycache == null)
            {
                throw new ArgumentNullException(nameof(IMemoryCache), "Post响应缓存依赖IMemoryCache，请services.AddMemoryCache()注入IMemoryCache后再使用[PostResponseCache]");
            }
            //标识已触发过响应缓存，防止中间件再次触发
            _memorycache.Set($"PostResponseCache_{context.HttpContext.Request.Path}", "标识已触发过响应缓存，防止中间件再次触发", new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) });

            var _requestCacheData = serviceProvider.GetService<RequestCacheData>();
            if (_requestCacheData == null)
            {
                throw new ArgumentNullException(nameof(IMemoryCache), "Post响应缓存依赖ResponseCacheData，请调用services.AddShareRequestBody()注入后再使用[PostResponseCache]");
            }

            var token = context.HttpContext.RequestAborted.Register(async () =>
            {
                await Task.CompletedTask;
                return;
            });

            var descriptor = (Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor)context.ActionDescriptor;

            var attribute = (IgnorePostResponseCacheAttribute)descriptor.MethodInfo.GetCustomAttributes(typeof(IgnorePostResponseCacheAttribute), true).FirstOrDefault();
            if (attribute != null)
            {
                //记录忽略的路由，中间件跳过
                _memorycache.Set($"IgnorePostResponseCache_{context.HttpContext.Request.Path}", true, new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) });
                goto gotoNext;
            }

            if (context.HttpContext.Request.Method.Equals("get", StringComparison.OrdinalIgnoreCase) || context.HttpContext.Request.Method.Equals("head", StringComparison.OrdinalIgnoreCase))
            {
                goto gotoNext;
            }
            else
            {
                var convertedDictionatry = context.HttpContext.Request.Query.ToDictionary(s => s.Key.ToLower(), s => s.Value);

                if (IgnoreVaryByQueryKeys == null || !IgnoreVaryByQueryKeys.Any())
                {
                    if (responseCacheOption.IgnoreVaryByQueryKeys.Any())
                    {
                        IgnoreVaryByQueryKeys = responseCacheOption.IgnoreVaryByQueryKeys.ToArray();
                    }
                }

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
                    bodyValue = await Common.ReadAsString(context.HttpContext);
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
                    ResponseCacheData cacheResponseBody = null;
                    if (Cluster)
                    {
                        cacheResponseBody = _redisManager.Get<ResponseCacheData>($"NetProPostResponse:{requestStrKey}");
                    }
                    else
                    {
                        cacheResponseBody = _memorycache.Get<ResponseCacheData>($"NetProPostResponse:{requestStrKey}");
                    }

                    if (cacheResponseBody != null && !context.HttpContext.RequestAborted.IsCancellationRequested)
                    {
                        if (!context.HttpContext.Response.HasStarted)
                        {
                            _logger.LogInformation($"触发PostResponseCacheAttribute本地缓存");
                            switch (ResponseMode)
                            {
                                case ResponseMode.Cache:
                                    context.HttpContext.Response.StatusCode = cacheResponseBody.StatusCode;
                                    context.HttpContext.Response.ContentType = cacheResponseBody.ContentType;
                                    await context.HttpContext.Response.WriteAsync(cacheResponseBody.Body);
                                    await Task.CompletedTask;
                                    return;
                                case ResponseMode.Error:
                                    if (cacheResponseBody.StatusCode == ((int)HttpStatusCode.OK))
                                    {
                                        //TODO确定StatusCode与Headers响应的先后顺序
                                        context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Conflict;
                                        context.HttpContext.Response.Headers.Add("Kestrel-ResponseMode", $"{ResponseMode}");
                                        context.HttpContext.Response.ContentType = cacheResponseBody.ContentType;
                                        await context.HttpContext.Response.WriteAsync(JsonSerializer.Serialize(new
                                        {
                                            Code = -1,
                                            Msg = $"{Message}"
                                        }, new JsonSerializerOptions
                                        {
                                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                                            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(System.Text.Unicode.UnicodeRanges.All)
                                        }), Encoding.UTF8);
                                        await Task.CompletedTask;
                                        return;
                                    }
                                    else
                                    {
                                        context.HttpContext.Response.StatusCode = cacheResponseBody.StatusCode;
                                        context.HttpContext.Response.ContentType = cacheResponseBody.ContentType;
                                        await context.HttpContext.Response.WriteAsync(cacheResponseBody.Body);
                                        await Task.CompletedTask;
                                        return; ;
                                    }
                            }
                        }
                        else
                        {
                            _logger.LogError($"StatusCode无法设置，因为响应已经启动,位置为:触发本地缓存开始赋值[responsecache2]");
                            await Task.CompletedTask;
                            return;
                        }
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

                            if (responseResult.GetType().Name == "EmptyResult")
                            {
                                await Task.CompletedTask;
                                return;
                            }

                            string body = string.Empty;
                            if (responseResult as Microsoft.AspNetCore.Mvc.ObjectResult != null)
                            {
                                body = JsonSerializer.Serialize(responseResult.Value, new JsonSerializerOptions
                                {
                                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(System.Text.Unicode.UnicodeRanges.All)
                                });
                            }

                            if (Cluster)
                            {
                                _redisManager.Set($"NetProPostResponse:{requestStrKey}", new ResponseCacheData
                                {
                                    Body = body,
                                    ContentType = "application/json",
                                    StatusCode = responseResult.StatusCode ?? 200,
                                }, TimeSpan.FromSeconds(Duration));
                            }
                            else
                            {
                                _memorycache.Set($"NetProPostResponse:{requestStrKey}", new ResponseCacheData
                                {
                                    Body = body,
                                    ContentType = "application/json",
                                    StatusCode = responseResult.StatusCode ?? 200,
                                }, TimeSpan.FromSeconds(Duration));
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "postResponse exception");
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

        private async Task<string> ReadAsString(HttpContext context, ILogger<PostResponseCacheAttribute> _iLogger)
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
            catch (Exception ex) when (!ex.Message?.Replace(" ", string.Empty).ToLower().Contains("unexpectedendofrequestcontent") ?? true)
            {
                _iLogger.LogError(ex, $"[ReadAsString] Post响应缓存读取body出错");
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
            catch (Exception)
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
