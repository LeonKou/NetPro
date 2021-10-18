using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetPro;
using NetPro.TypeFinder;
using NetPro.Web.Core.Helpers;
using NetPro.Web.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Leon.XXXV2.Api
{
    public class ApiStartup : INetProStartup
    {
        public double Order => 900;

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration = null, ITypeFinder typeFinder = null)
        {
            //var controllerAssemblyname = typeof(ControllerBase).Assembly.GetName().Name;
            //var mvcBuilder = services.AddControllers();
            //foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            //{
            //    if (assembly.GetReferencedAssemblies().Any(x => x.Name == controllerAssemblyname))
            //        mvcBuilder.AddApplicationPart(assembly);
            //}
        }

        public void Configure(IApplicationBuilder app)
        {
            //app.UseExceptionHandler(handler =>
            //{
            //    handler.Run(async context =>
            //    {
            //        var webHelper = handler.ApplicationServices.GetRequiredService<IWebHelper>();
            //        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            //        context.Response.ContentType = "application/json;charset=utf-8";
            //        var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
            //        if (contextFeature != null)
            //        {
            //            var exceptionHandlerPathFeature =
            //            context.Features.Get<IExceptionHandlerPathFeature>();
            //            if (exceptionHandlerPathFeature?.Error != null)
            //            {
            //                if (!exceptionHandlerPathFeature.Error.Message?.Replace(" ", string.Empty).ToLower().Contains("unexpectedendofrequestcontent") ?? true)
            //                {
            //                    string body = null;
            //                    string userInfo = context?.User.Identity.Name;

            //                    if (context?.Request.Method.ToUpper() == "POST")
            //                    {
            //                        try
            //                        {
            //                            context.Request.Body.Position = 0;
            //                            using (StreamReader reader = new StreamReader(context?.Request.Body, Encoding.UTF8, true, 1024, true))
            //                            {
            //                                body = await reader.ReadToEndAsync();
            //                            }
            //                        }
            //                        catch (Exception ex)
            //                        {
            //                            logger.LogError($"Global exception capture is error for reads the body", ex);
            //                        }
            //                    }
            //                    //logger.LogError(exceptionHandlerPathFeature?.Error, @$"[{DateTime.Now:HH:mm:ss}] [Global system exception]
            //                    //    RequestIp=> {webHelper?.GetCurrentIpAddress()}
            //                    //    HttpMethod=> {context?.Request.Method}
            //                    //    Path=> {context.Request.Host.Value}{context?.Request.Path}{context?.Request.QueryString}
            //                    //    Body=> {body}
            //                    //    Header=> 
            //                    //    { string.Join("\r\n", context?.Request.Headers.ToList())}
            //                    //    UserId=> {userInfo}
            //                    //    ");
            //                }
            //            }
            //            context.Response.Headers.Add("error", $"{exceptionHandlerPathFeature?.Error.Message}");
            //            await context.Response.WriteAsync(JsonSerializer.Serialize(new ResponseResult { Code = -1, Msg = $"System exception, please try again later", Result = "" }, new JsonSerializerOptions
            //            {
            //                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            //                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(System.Text.Unicode.UnicodeRanges.All)
            //            }), Encoding.UTF8);
            //            await Task.CompletedTask;
            //            return;
            //        }
            //    });
            //});

            //app.UseDeveloperExceptionPage();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
