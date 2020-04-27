using NetPro.Core.Consts;
using NetPro.Core.Infrastructure;
using NetPro.Utility;
using NetPro.Utility.Helpers;
using NetPro.Web.Core.Filters;
using NetPro.Web.Core.Helpers;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace NetPro.Web.Core.Middlewares
{
	/// <summary>
	/// 签名验证
	/// </summary>
	public class VerifySignAttribute : Attribute, IAsyncActionFilter
	{
		private readonly ILogger _logger;
		readonly IConfiguration _configuration;

		/// <summary>
		/// 
		/// </summary>
		public VerifySignAttribute()
		{
			_configuration = EngineContext.Current.Resolve<IConfiguration>();
			_logger = EngineContext.Current.Resolve<ILogger>();
		}

		private bool HasVerifySignAttribute(Type t)
		{
			if (t.FullName == "Microsoft.AspNetCore.Mvc.Controller") return false;
			if (t.GetCustomAttributes(typeof(VerifySignAttribute), true).Any())
			{
				return true;
			}
			else
			{
				return HasVerifySignAttribute(t.BaseType);
			}
		}

		public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
		{
			var isOpenVerifySign = _configuration.GetValue("IsOpenVerifySign", false);
			var descriptor = (Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor)context.ActionDescriptor;
			var hasIgnoreattribute = descriptor.MethodInfo.GetCustomAttributes(typeof(IgnoreSignAttribute), true).Any();
			var hasVerifySignAttribute = descriptor.MethodInfo.GetCustomAttributes(typeof(VerifySignAttribute), true).Any();

			//方法不加鉴权特性并且鉴权打开并且有忽略或者方法不加鉴权并且鉴权关闭统一都忽略鉴权否则打开鉴权---方法加鉴权>鉴权开关
			if (!hasVerifySignAttribute && ((isOpenVerifySign && hasIgnoreattribute) || !isOpenVerifySign))
				goto gotoNext;

			string appId = null;
			string bodyText = null;
			try
			{
				var request = context.HttpContext.Request;
				bodyText = TryPassJsonToQueryString(ActionFilterHelper.GetRequestBodyText(request)?.Trim());
				var allKeys = new Dictionary<string, object>();
				if (!string.IsNullOrWhiteSpace(bodyText))
				{
					allKeys = bodyText.TrimStart('?').Split('&').Select(a => new
					{
						Key = a.Substring(0, a.IndexOf('=')).ToLower(),
						Value = (object)a.Substring(a.IndexOf('=') + 1)
					}).ToDictionary(a => a.Key, a => (object)HttpUtility.UrlDecode(a.Value.ToString()));
				}

				if (request.Headers != null && request.Headers.Keys.Any())
				{
					var headers = request.Headers.ToDictionary(a => a.Key.ToLower(), a => a.Value).Where(a => new[] { "appid", "sign" }.Contains(a.Key)).ToDictionary(a => a.Key, a => a.Value.ToString());
					if (headers.Any())
					{
						foreach (var header in headers)
						{
							if (allKeys.ContainsKey(header.Key))
								allKeys[header.Key] = header.Value;
							else
								allKeys.Add(header.Key, header.Value);
						}
					}
				}

				if (request.Query != null && request.Query.Keys.Any())
				{
					foreach (var key in request.Query.Keys)
					{
						var value = HttpUtility.UrlDecode(request.Query[key]);
						var lowerKey = key.ToLower();
						if (allKeys.ContainsKey(lowerKey))
							allKeys[lowerKey] = value;
						else
							allKeys.Add(lowerKey, value);
					}
				}

				if (!allKeys.ContainsKey("appid") || !allKeys.ContainsKey("sign"))
				{
					BuildErrorJson(context, "不存在AppId/Sign", bodyText);
					return;
				}

				var secrets = new List<AppSecret>();
				_configuration.GetSection("Secrets").Bind(secrets);
				appId = (string)allKeys["appid"];
				var secret = secrets.FirstOrDefault(a => a.AppId == appId)?.Secret;
				//获取两份签名，一份原始数据，一份忽略大小写的签名。然后用两份签名比较
				var upperSign = SignHelper.GetSign(allKeys, secret);
				var lowerSign = SignHelper.GetSign(allKeys, secret, true);
				var sign = (string)allKeys["sign"];
				if (upperSign != sign && lowerSign != sign)
				{
					BuildErrorJson(context, $"签名不正确：sign：{sign}，upperSign：{upperSign}，lowerSign：{lowerSign}", bodyText);
					return;
				}
			}
			catch (Exception ex)
			{
				BuildErrorJson(context, "签名验证失败!" + ex.Message, bodyText);
				return;
			}

			gotoNext:
			await next();
		}

		private void BuildErrorJson(ActionExecutingContext context, string errorMsg, string body)
		{
			var url = HttpUtility.UrlDecode(UriHelper.GetDisplayUrl(context.HttpContext.Request));
			_logger.Warning($"签名认证失败，ErrorMsg：{errorMsg}，url：{url}，body：{body}");
			context.HttpContext.Response.StatusCode = 200;
			context.HttpContext.Response.ContentType = "application/json";
			context.Result = "签名验证失败!".ToErrorActionResult(AppErrorCode.VerifySignFailt.Value());
		}

		private string TryPassJsonToQueryString(string json)
		{
			string queryString = json;
			if (string.IsNullOrEmpty(json)) return queryString;
			try
			{
				var dic = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
				queryString = string.Join("&", dic.Select(a => $"{a.Key.Trim()}={HttpUtility.UrlEncode(Regex.Replace(a.Value ?? string.Empty, "\\s*", string.Empty))}"));
			}
			catch { }
			return queryString;
		}
	}
}
