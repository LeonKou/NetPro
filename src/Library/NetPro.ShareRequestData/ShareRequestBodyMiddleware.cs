using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace NetPro.ShareRequestBody
{
    public class ShareRequestBodyMiddleware
    {
        private readonly RequestDelegate _next;

        public ShareRequestBodyMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="responseCacheData">自定义对象不能ctor注入</param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext context, RequestCacheData requestCacheData)
        {
            context.Request.EnableBuffering();
            var token = context.RequestAborted.Register(async () =>
            {
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
                string bodyValue;
                if (requestCacheData == null || string.IsNullOrEmpty(requestCacheData.Body))
                {
                    bodyValue = await Common.ReadAsString(context);
                    requestCacheData = new RequestCacheData
                    {
                        Body = bodyValue
                    };
                }
            }
        }
    }

    public static class ShareRequestBodyMiddlewareExtensions
    {
        /// <summary>
        /// 使用共享请求体组件
        /// 建议尽量放于请求管道中的最上层
        /// </summary>
        /// <param name="app"></param>
        public static IApplicationBuilder UseShareRequestBody(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ShareRequestBodyMiddleware>();
        }
    }
}
