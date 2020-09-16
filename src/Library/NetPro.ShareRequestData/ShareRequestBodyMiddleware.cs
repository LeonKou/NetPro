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
        /// <param name="requestCacheData"></param>
        /// <returns></returns>
        /// <remarks>错误： OnStarting cannot be set because the response has already started.</remarks>
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

