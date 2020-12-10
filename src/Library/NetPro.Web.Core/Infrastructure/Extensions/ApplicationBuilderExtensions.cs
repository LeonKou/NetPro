using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NetPro.Core.Configuration;
using NetPro.Core.Infrastructure;
using NetPro.Utility;
using NetPro.Web.Core.Helpers;
using NetPro.Web.Core.Models;
using Serilog;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace NetPro.Web.Core.Infrastructure.Extensions
{
    /// <summary>
    ///IApplicationBuilder扩展
    /// </summary>
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// 配置http 请求管道
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public static void ConfigureRequestPipeline(this IApplicationBuilder application)
        {
            EngineContext.Current.ConfigureRequestPipeline(application);
        }

        /// <summary>
        /// Adds a special handler that checks for responses with the 400 status code (bad request)
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public static void UseBadRequestResult(this IApplicationBuilder application)
        {
            application.UseStatusCodePages(context =>
            {
                //handle 404 (Bad request)
                if (context.HttpContext.Response.StatusCode == StatusCodes.Status400BadRequest)
                {
                    var logger = EngineContext.Current.Resolve<ILogger>();
                    logger.Error($"Error 400. Bad request,{context.HttpContext.Request.Path.Value}");
                }

                return Task.CompletedTask;
            });
        }

        /// <summary>
        /// Add exception handling
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public static void UseNetProExceptionHandler(this IApplicationBuilder application)
        {
            var nopConfig = application.ApplicationServices.GetService<NetProOption>();
            var hostingEnvironment = application.ApplicationServices.GetService<IWebHostEnvironment>();
            var requestIp = application.ApplicationServices.GetService<IWebHelper>();

            if (!hostingEnvironment.IsDevelopment())
            {
                //全局默认异常捕获(响应被处理，此处将不再处理)
                application.UseExceptionHandler(handler =>
                {
                    handler.Run(async context =>
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        context.Response.ContentType = "application/json;charset=utf-8";
                        var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                        if (contextFeature != null)
                        {
                            var exceptionHandlerPathFeature =
                   context.Features.Get<IExceptionHandlerPathFeature>();
                            if (exceptionHandlerPathFeature?.Error != null)
                            {
                                if (!exceptionHandlerPathFeature.Error.Message?.Replace(" ", string.Empty).ToLower().Contains("unexpectedendofrequestcontent") ?? true)
                                {
                                    var url = string.Format("{0}{1}", context.Request.Host.Value, context.Request.Path.Value);
                                    Serilog.Log.Error(exceptionHandlerPathFeature?.Error, $"系统异常, requestIp: {requestIp} url:  {url}");
                                }
                            }

                            await context.Response.WriteAsync(JsonSerializer.Serialize(new ResponseResult { Code = -1, Msg = $"系统异常,请稍后再试", Result = "" }, new JsonSerializerOptions
                            {
                                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(System.Text.Unicode.UnicodeRanges.All)
                            }), Encoding.UTF8);
                            await Task.CompletedTask;
                            return;
                        }
                    });
                });

            }
            if (hostingEnvironment.IsDevelopment())
            {
                //get detailed exceptions for developing and testing purposes
                application.UseDeveloperExceptionPage();
            }
        }

        /// <summary>
        /// Adds a special handler that checks for responses with the 404 status code that do not have a body
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public static void UsePageNotFound(this IApplicationBuilder application)
        {
            application.UseStatusCodePages(async context =>
            {
                //handle 404 Not Found
                if (context.HttpContext.Response.StatusCode == StatusCodes.Status404NotFound)
                {
                    var webHelper = EngineContext.Current.Resolve<IWebHelper>();
                    if (!webHelper.IsStaticResource())
                    {
                        //get original path and query
                        var originalPath = context.HttpContext.Request.Path;
                        var originalQueryString = context.HttpContext.Request.QueryString;

                        //store the original paths in special feature, so we can use it later
                        context.HttpContext.Features.Set<IStatusCodeReExecuteFeature>(new StatusCodeReExecuteFeature()
                        {
                            OriginalPathBase = context.HttpContext.Request.PathBase.Value,
                            OriginalPath = originalPath.Value,
                            OriginalQueryString = originalQueryString.HasValue ? originalQueryString.Value : null,
                        });
                        var config = EngineContext.Current.Resolve<NetProOption>();
                        var pageNotFoundUrl = config.PageNotFoundUrl;
                        if (string.IsNullOrWhiteSpace(pageNotFoundUrl))
                        {
                            return;
                        }
                        context.HttpContext.Request.Path = pageNotFoundUrl;
                        context.HttpContext.Request.QueryString = QueryString.Empty;
                        try
                        {
                            //re-execute request with new path
                            await context.Next(context.HttpContext);
                        }
                        finally
                        {
                            //return original path to request
                            context.HttpContext.Request.QueryString = originalQueryString;
                            context.HttpContext.Request.Path = originalPath;
                            context.HttpContext.Features.Set<IStatusCodeReExecuteFeature>(null);
                        }
                    }
                }
            });
        }

    }
}
