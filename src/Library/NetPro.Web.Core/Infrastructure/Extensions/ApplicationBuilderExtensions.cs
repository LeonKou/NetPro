using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using NetPro.Core.Infrastructure;
using NetPro.Core.Configuration;
using Microsoft.Extensions.Hosting;
using NetPro.Core;
using Serilog;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using NetPro.Core.Consts;
using NetPro.Web.Core;
using System.IO;
using System.Text;
using System.Net;
using NetPro.Utility;
using NetPro.Web.Core.Models;
using NetPro.Web.Core.Helpers;

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
			var nopConfig = EngineContext.Current.Resolve<NetProOption>();
			var hostingEnvironment = EngineContext.Current.Resolve<IWebHostEnvironment>();
			var useDetailedExceptionPage = nopConfig.DisplayFullErrorStack || hostingEnvironment.IsDevelopment();
			//var useDetailedExceptionPage = nopConfig.DisplayFullErrorStack;
			if (useDetailedExceptionPage)
			{
				//get detailed exceptions for developing and testing purposes
				application.UseDeveloperExceptionPage();
			}
			else
			{
				//or use special exception handler
				application.UseExceptionHandler("/errorpage.htm");
			}
			//全局默认异常捕获(响应被处理，此处将不再处理)
			application.UseExceptionHandler(handler =>
			{
				handler.Run(async context =>
				{
					var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
					if (exception != null)
					{
						context.Response.StatusCode = 500;
						context.Response.ContentType = "application/json";
						string errorMsg = "程序访问异常,请稍后重试！";

						string requestPara = string.Empty;
						var request = context.Request;
						var method = request.Method.ToUpper();
						if (method == "POST" || method == "PUT" || method == "DELETE")
						{
							request.Body.Position = 0;
							using (StreamReader reader = new StreamReader(request.Body, Encoding.UTF8))
							{
								requestPara = reader.ReadToEnd();
							}
						}
						int errorCode = 1;
						string requestId = string.Empty;

						if (exception is NetProException)
						{
							var NetProEx = (NetProException)exception;
							errorMsg = NetProEx.Message;
							errorCode = NetProEx.ErrorCode;
							requestId = NetProEx.RequestId;
						}
						//写入日志系统
						var url = string.Format("{0}{1}", request.Host.Value, request.Path.Value);
						var requestIp = EngineContext.Current.Resolve<IWebHelper>().GetCurrentIpAddress();
						Serilog.Log.Error(exception, "WebApi异常.errorCode:{0},requestId:{1},请求url:{2},参数:{3},IP:{4}", errorCode, requestId, url, requestPara, requestIp);
						//自定义异常返回
						await context.Response.WriteAsync(JsonConvert.SerializeObject(new ApiResultModel()
						{
							ErrorCode = errorCode,
							Msg = errorMsg
						})).ConfigureAwait(false);
					}
				});
			});
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
