using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace NetPro.ShareRequestBody
{
    /// <summary>
    /// 
    /// </summary>
    public class ShareRequestBodyMiddleware
    {
        private readonly ILogger _logger;
        private readonly RequestDelegate _next;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="next"></param>
        /// <param name="logger"></param>
        public ShareRequestBodyMiddleware(RequestDelegate next
            , ILogger<ShareRequestBodyMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="requestCacheData"></param>
        /// <returns></returns>
        /// <remarks>错误： OnStarting cannot be set because the response has already started.</remarks>
        public async Task InvokeAsync(HttpContext context, RequestCacheData requestCacheData)
        {
            context.Request.EnableBuffering();
            var token = context.RequestAborted.Register(async () =>
            {
                await Task.CompletedTask;
                return;
            });

            if (context.Request.ContentType?.Contains("multipart/form-data") ?? false || context.Request.Method.Equals("get", StringComparison.OrdinalIgnoreCase) || context.Request.Method.Equals("head", StringComparison.OrdinalIgnoreCase))
            {
                await _next(context);
            }
            else
            {
                string bodyValue;
                bodyValue = await Common.ReadAsString(context);
                if (requestCacheData == null)
                {
                    requestCacheData = new RequestCacheData { Body = bodyValue };
                }
                else if (string.IsNullOrEmpty(requestCacheData.Body))
                {
                    requestCacheData.Body = bodyValue;
                    await _next(context);
                }
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public static class ShareRequestBodyMiddlewareExtensions
    {
        /// <summary>
        /// 共享请求体组件
        /// 放于UseRouting之后
        /// 建议尽量放于请求管道中的最上层
        /// 实际压测数据显示，增加body共享的处理比每次解析body花费时间更久，再body解析不超过5次情况下，谨慎使用body共享组件
        /// </summary>
        /// <param name="builder"></param>
        public static IApplicationBuilder UseShareRequestBody(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ShareRequestBodyMiddleware>();
        }
    }
}

