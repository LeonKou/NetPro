/*
 *  MIT License
 *  
 *  Copyright (c) 2021 LeonKou
 *  
 *  Permission is hereby granted, free of charge, to any person obtaining a copy
 *  of this software and associated documentation files (the "Software"), to deal
 *  in the Software without restriction, including without limitation the rights
 *  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 *  copies of the Software, and to permit persons to whom the Software is
 *  furnished to do so, subject to the following conditions:
 *  
 *  The above copyright notice and this permission notice shall be included in all
 *  copies or substantial portions of the Software.
 *  
 *  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 *  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 *  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 *  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 *  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 *  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 *  SOFTWARE.
 */

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NetPro.CsRedis;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace NetPro.Analysic
{
    /// <summary>
    /// 
    /// </summary>
    public class RequestAnalysisMiddleware
    {
        private readonly ILogger _iLogger;
        private readonly RequestAnalysisOption _requestAnalysisOption;
        private readonly RequestDelegate _next;
        private readonly IMemoryCache _memorycache;
        private readonly IRedisManager _redisManager;
        private readonly RedisCacheOption _redisCacheOption;
        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="next"></param>
        /// <param name="iLogger"></param>
        /// <param name="requestAnalysisOption"></param>
        /// <param name="memorycache"></param>
        /// <param name="redisManager"></param>
        /// <param name="redisCacheOption"></param>
        /// <param name="httpContextAccessor"></param>
        public RequestAnalysisMiddleware(RequestDelegate next,
            ILogger<RequestAnalysisMiddleware> iLogger,
            RequestAnalysisOption requestAnalysisOption,
            IMemoryCache memorycache,
            IRedisManager redisManager,
            RedisCacheOption redisCacheOption,
            IHttpContextAccessor httpContextAccessor)
        {
            _next = next;
            _iLogger = iLogger;
            _requestAnalysisOption = requestAnalysisOption;
            _memorycache = memorycache;
            _redisManager = redisManager;
            _redisCacheOption = redisCacheOption;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext context)
        {
            var token = context.RequestAborted.Register(async () =>
            {
                await Task.CompletedTask;
                return;
            });

            var analysisConfigListTemp = _requestAnalysisOption.PolicyOption;
            _iLogger.LogDebug($"流量分析获取的请求路径:{context.Request.Path.Value}"); //TODO 支持包含{id}此类路径
            var analysisConfigTemp = analysisConfigListTemp?.Where(s => s.Enabled == true && s.Path.Equals(context.Request.Path.Value, StringComparison.OrdinalIgnoreCase))?.FirstOrDefault();

            int maxLimitTemp = 0;
            int maxErrorLimitTemp = 0;
            if (analysisConfigTemp == null)
            {
                goto gotoNext;
            }
            if (analysisConfigTemp != null && maxLimitTemp == 0)
            {
                maxLimitTemp = analysisConfigTemp.MaxSucceedLimit;
            }
            if (analysisConfigTemp != null && maxErrorLimitTemp == 0)
            {
                maxErrorLimitTemp = analysisConfigTemp.MaxErrorLimit;
            }
            string path = context.Request.Path.Value.Replace('/', ':');

            string limitkey = $"{RedisKeys.Key_Request_Limit_IP_Lock}{GetRequestIp()}";
            if (!analysisConfigTemp.IsGlobalLock)
            {
                limitkey = limitkey + path;
            }

            if (_redisManager.Exists(limitkey))
            {
                _iLogger.LogWarning($"[Analysic] 触发请求锁定 IsGlobalLock：{analysisConfigTemp.IsGlobalLock}  时长：{analysisConfigTemp.LockDuration} ");
                await BuildErrorJson(context);
                await Task.CompletedTask;
                return;
            }
            var CheckIpValidResultByConfig = CheckIpValid(maxLimitTemp, maxErrorLimitTemp, context.Request.Path.Value.Replace('/', ':'));
            if (!CheckIpValidResultByConfig.Item1)
            {
                //触发限制
                if (!string.IsNullOrWhiteSpace(analysisConfigTemp.LockDuration))
                {
                    _redisManager.GetOrSet($"{limitkey}", () =>
                    {
                        return context.Request.QueryString.Value;
                    }, _ConvertToTimeSpan(analysisConfigTemp.LockDuration), (int)_ConvertToTimeSpan(analysisConfigTemp.LockDuration).TotalSeconds - 1);
                }

                _iLogger.LogWarning($"[Analysic] 触发请求限制 时长：{analysisConfigTemp.HitDuration} ");
                await BuildErrorJson(context);
                await Task.CompletedTask;
                return;
            }
            else
            {
                await _next(context);

                var currentIp = GetRequestIp();
                if (context.Response.StatusCode == 200)
                {
                    string keySucceed = $"{RedisKeys.Key_Request_Limit_IP_Succ}{currentIp}";
                    RecordRequest(keySucceed: $"{keySucceed}{path}", hitDuration: analysisConfigTemp.HitDuration);
                }
                else
                {
                    string keyError = $"{RedisKeys.Key_Request_Limit_IP_Error}{currentIp}";
                    RecordRequest(keyError: $"{keyError}{path}", hitDuration: analysisConfigTemp.HitDuration);
                }

                await Task.CompletedTask;
                return;
            }
        gotoNext:
            await _next(context);
        }

        private async Task BuildErrorJson(HttpContext context)
        {
            if (!context?.Response.HasStarted ?? false)
            {
                _iLogger.LogWarning($"[HighRiskIP]IP高危：{GetRequestIp()} 请求警告");
                context.Response.StatusCode = (int)HttpStatusCode.Conflict;
                context.Response.ContentType = "text/plain; charset=utf-8";
                context.Response.Headers["WARNING"] = "NetPro.Analysic";

                await context.Response.WriteAsync("您的IP已被风控");
                //JsonSerializer.Serialize(new
                //{
                //    Code = -1,
                //    Msg = $"您的IP已被风控"
                //}), Encoding.UTF8);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="maxLimit"></param>
        /// <param name="maxErrorLimit"></param>
        /// <param name="path"></param>
        /// <returns>
        /// item1:请求是否合法，true合法；false不合法
        /// item2:请求成功的次数
        /// item3:请求失败的次数
        /// </returns>
        private Tuple<bool, int, int> CheckIpValid(int maxLimit, int maxErrorLimit, string path = null)
        {
            //TODO 优化到redis内脚本判断
            //同ip请求次数限制
            //同用户请求次数限制
            //TODO 同IP 每小时关联的用户数
            //TODO 同IP每小时关联的UUID数             
            var currentIp = GetRequestIp();
            int limitByIp = 0;
            int limitByIpError = 0;

            //校验成功请求次数
            string keySucceed = $"{RedisKeys.Key_Request_Limit_IP_Succ}{currentIp}";
            if (!string.IsNullOrEmpty(path))
            {
                keySucceed = $"{keySucceed }{path}";
            }

            limitByIp = _redisManager.Get<int>(keySucceed);

            //校验失败请求次数
            string keyError = $"{RedisKeys.Key_Request_Limit_IP_Error}{currentIp}";
            if (!string.IsNullOrEmpty(path))
            {
                keyError = $"{keyError}{path}";
            }
            limitByIpError = _redisManager.Get<int>(keyError);

            if (maxLimit > 0 && limitByIp >= maxLimit)
                return Tuple.Create<bool, int, int>(false, limitByIp, limitByIpError);

            if (maxErrorLimit > 0 && limitByIpError >= maxErrorLimit)
                return Tuple.Create<bool, int, int>(false, limitByIp, limitByIpError);

            return Tuple.Create<bool, int, int>(true, limitByIp, limitByIpError);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string GetRequestIp()
        {
            string stringIp = string.Empty;
            if (_httpContextAccessor.HttpContext.Request.Headers.ContainsKey("X-Real-IP"))
            {
                stringIp = _httpContextAccessor.HttpContext.Request.Headers["X-Real-IP"];
                _iLogger.LogInformation($"[GetRequestIp]请求IP--X-Real-IP-->{stringIp }");
                return stringIp;
            }
            else if (_httpContextAccessor.HttpContext.Request.Headers.ContainsKey("X-Forwarded-For"))
            {
                stringIp = _httpContextAccessor.HttpContext.Request.Headers["X-Forwarded-For"];
                _iLogger.LogInformation($"[GetRequestIp]请求IP--X-Forwarded-For-->{stringIp }");
                return stringIp;
            }
            else
            {
                stringIp = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                _iLogger.LogInformation($"[GetRequestIp]请求IP--RemoteIpAddress-->{stringIp }");
                return stringIp;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keySucceed"></param>
        /// <param name="keyError"></param>
        /// <param name="hitDuration"></param>
        private void RecordRequest(string keySucceed = null, string keyError = null, string hitDuration = "1d")
        {
            if (!string.IsNullOrEmpty(keySucceed))
            {
                if (!_redisManager.Exists(keySucceed))
                {
                    _redisManager.StringIncrement(keySucceed, expiry: _ConvertToTimeSpan(hitDuration));
                }
                else
                {
                    _redisManager.StringIncrement(keySucceed);
                }
            }
            else
            {
                if (!_redisManager.Exists(keyError))
                {
                    _redisManager.StringIncrement(keyError, expiry: _ConvertToTimeSpan(hitDuration));
                }
                else
                {
                    _redisManager.StringIncrement(keyError);
                }
            }
        }

        private TimeSpan _ConvertToTimeSpan(string timeSpan)
        {
            var l = timeSpan.Length - 1;
            var value = timeSpan.Substring(0, l);
            var type = timeSpan.Substring(l, 1);

            switch (type)
            {
                case "d": return TimeSpan.FromDays(double.Parse(value));
                case "h": return TimeSpan.FromHours(double.Parse(value));
                case "m": return TimeSpan.FromMinutes(double.Parse(value));
                case "s": return TimeSpan.FromSeconds(double.Parse(value));
                case "f": return TimeSpan.FromMilliseconds(double.Parse(value));
                case "z": return TimeSpan.FromTicks(long.Parse(value));
                default: return TimeSpan.FromDays(double.Parse(value));
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public static class RequestAnalysisMiddlewareExtensions
    {
        /// <summary>
        /// 请求分析
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>  
        /// <remarks></remarks>
        public static IApplicationBuilder UseRequestAnalysis(
            this IApplicationBuilder builder)
        {
            var responseCacheOption = builder.ApplicationServices.GetService<RequestAnalysisOption>();
            if (responseCacheOption?.Enabled ?? false)
            {
                if (builder.ApplicationServices.GetService<IRedisManager>() == null || builder.ApplicationServices.GetService<IMemoryCache>() == null)
                    return builder;
                builder.UseMiddleware<RequestAnalysisMiddleware>();
            }

            return builder;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    internal static class BodyCommonTools
    {
        public static async Task<string> FormatRequestBody(HttpRequest request)
        {
            //This line allows us to set the reader for the request back at the beginning of its stream.
            request.EnableBuffering();

            //We now need to read the request stream.  First, we create a new byte[] with the same length as the request stream...
            var buffer = new byte[Convert.ToInt32(request.ContentLength)];

            //...Then we copy the entire request stream into the new buffer.
            await request.Body.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false);

            //We convert the byte[] into a string using UTF8 encoding...
            var bodyAsText = Encoding.UTF8.GetString(buffer);

            //..and finally, assign the read body back to the request body, which is allowed because of EnableRewind()
            request.Body.Position = 0;

            return $"{request.Scheme} {request.Host}{request.Path} {request.QueryString} {bodyAsText}";
        }

        public static async Task<string> FormatResponseBody(HttpResponse response)
        {
            //We need to read the response stream from the beginning...
            response.Body.Seek(0, SeekOrigin.Begin);

            //...and copy it into a string
            string text = await new StreamReader(response.Body).ReadToEndAsync();

            //We need to reset the reader for the response so that the client can read it.
            response.Body.Seek(0, SeekOrigin.Begin);

            response.Body.Position = 0;

            //Return the string for the response, including the status code (e.g. 200, 404, 401, etc.)
            return text;
        }
    }
}
