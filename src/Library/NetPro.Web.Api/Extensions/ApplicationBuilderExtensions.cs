using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.NetPro;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace NetPro.Web.Api
{
    /// <summary>
    ///IApplicationBuilder扩展
    /// </summary>
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Add exception handling
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public static void UseNetProExceptionHandler(this IApplicationBuilder application)
        {
            var netConfig = application.ApplicationServices.GetService<NetProOption>();
            var hostingEnvironment = application.ApplicationServices.GetService<IWebHostEnvironment>();

            var logger = application.ApplicationServices.GetRequiredService<Microsoft.Extensions.Logging.ILogger<dynamic>>();

            if (hostingEnvironment.IsDevelopment())
            {
                //get detailed exceptions for developing and testing purposes
                application.UseDeveloperExceptionPage();
            }
            else
            {
                {
                    var webHelper = application.ApplicationServices.GetService<IWebHelper>();
                    //The global default handles exceptions
                    application.UseExceptionHandler(handler =>
                    {
                        handler.Run(async context =>
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                            context.Response.ContentType = "text/plain; charset=utf-8";
                            var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                            if (contextFeature != null)
                            {
                                //var exceptionHandlerPathFeature =
                                //context.Features.Get<IExceptionHandlerPathFeature>();
                                if (contextFeature.Error != null)
                                {
                                    //Microsoft.AspNetCore.Server.Kestrel.Core.BadHttpRequestException
                                    var exError = contextFeature.Error;

                                    if (!exError.Message?.Replace(" ", string.Empty).ToLower().Contains("unexpectedendofrequestcontent") ?? true)
                                    {
                                        string body = null;
                                        string userInfo = context?.User.Identity.Name;

                                        if (context?.Request.Method.ToUpper() == "POST")
                                        {
                                            try
                                            {
                                                context.Request.Body.Position = 0;
                                                using (StreamReader reader = new StreamReader(context?.Request.Body, Encoding.UTF8, true, 1024, true))
                                                {
                                                    body = await reader.ReadToEndAsync();
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                logger.LogError($"Global exception capture is error for reads the body", ex);
                                            }
                                        }
                                        logger.LogError(exError, @$"[{DateTime.Now:HH:mm:ss}] [Global system exception]
                                    RequestIp=> {webHelper?.GetCurrentIpAddress()}
                                    HttpMethod=> {context?.Request.Method}
                                    Path=> {context.Request.Host.Value}{context?.Request.Path}{context?.Request.QueryString}
                                    Body=> {body}
                                    Header=> 
                                    { string.Join("\r\n", context?.Request.Headers.ToList())}
                                    UserId=> {userInfo}
                                    ");
                                    }
                                }
                                if (netConfig.IsDebug)
                                {
                                    context.Response.Headers.Add("error", $"{contextFeature.Error.Message}");
                                }
                                
                                await context.Response.WriteAsync("System exception, please try again later");
                                //    JsonSerializer.Serialize(new ResponseResult { Code = -1, Msg = $"System exception, please try again later", Result = "" }, new JsonSerializerOptions
                                //{
                                //    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                                //    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(System.Text.Unicode.UnicodeRanges.All)
                                //}), Encoding.UTF8);
                                await Task.CompletedTask;
                                return;
                            }
                        });
                    });

                }
            }
        }

        /// <summary>
        /// Adds a special handler that checks for responses with the 404 status code that do not have a body
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public static void UsePageNotFound(this IApplicationBuilder application)
        {
            //var webHelper = application.ApplicationServices.GetService<IWebHelper>();//启动前执行到此IWebHelper还未创建，因为他是scope的
            application.UseStatusCodePages(async context =>
            {
                //handle 404 Not Found
                if (context.HttpContext.Response.StatusCode == StatusCodes.Status404NotFound)
                {
                    var webHelper = context.HttpContext.RequestServices.GetService<IWebHelper>();
                    //var webHelper = EngineContext.Current.Resolve<IWebHelper>();
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
