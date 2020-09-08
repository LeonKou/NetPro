using NetPro.Web.Core.Helpers;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NetPro.Web.Core;
using NetPro.Core.Consts;
using NetPro.Utility;
using NetPro.Web.Core.Permission;
using System.Linq;
using NetPro.Core.Infrastructure;
using NetPro.Core.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Serilog;

namespace NetPro.Web.Core.Filters
{
	/// <summary>
	/// 权限控制过滤器
	/// </summary>
	public class PermissionActionFilter : IAsyncActionFilter
	{
		readonly NetProOption _config;
		readonly INetProPermissionService _NetProPermissionService;
		readonly ILogger _logger;

		public PermissionActionFilter(NetProOption config, INetProPermissionService NetProPermissionService, ILogger logger)
		{
			_config = config;
			_NetProPermissionService = NetProPermissionService;
			_logger = logger;
		}

		public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
		{
			var descriptor = (Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor)context.ActionDescriptor;
			var controllerName = descriptor.ControllerName;
			var actionName = descriptor.ActionName;
			var identity = context.HttpContext.User.Identity;
			long? userId;
			if (identity.IsAuthenticated)
			{
				userId = identity.GetUserId<long>();
			}
			var roleName = identity.GetRoleName();

			var permissionAttribute = (PermissionActionAttribute)descriptor.MethodInfo.GetCustomAttributes(typeof(PermissionActionAttribute), true).FirstOrDefault();
			// 过滤匿名访问
			//if (context.Filters.Any(item => item is IAllowAnonymousFilter) || ((!(_config.PermissionEnabled) && string.IsNullOrWhiteSpace(_config.Permission)) || (permissionAttribute?.Ignore).GetValueOrDefault()))
			//{
			//	await next();
			//	return;
			//}

			string redirectUrl = "";
			var ret = false;//是否拥有权限

			if (_config.Permission == "url")//通过url判断权限
			{
				ret = _NetProPermissionService.HasPermission(controllerName, actionName);
			}
			else
			{
				if (permissionAttribute != null)
				{
					redirectUrl = permissionAttribute.RedirectUrl;
					string programCode = permissionAttribute.ProgramCode;
					ret = _NetProPermissionService.HasPermission(programCode);
				}
			}
			if (!ret)
			{
				_logger.Warning($"权限不足---{context.HttpContext.User.Identity.Name}用户无权访问---{controllerName}/{actionName}");
				if (_config.AppType == AppType.Api)
				{
					string errorMsg = "用户无权访问!";
					throw new NetProException(errorMsg, AppErrorCode.PermissionDenied.Value());
				}
				else
				{
					if (string.IsNullOrEmpty(redirectUrl))
					{
						redirectUrl = _config.LoginUrl;
					}
					if (string.IsNullOrWhiteSpace(redirectUrl))
					{
						context.Result = new ContentResult() { Content = "您暂无权限操作" };
					}
					else
					{
						context.Result = new RedirectResult(redirectUrl);
					}
				}
				return;
			}
			await next();
		}
	}
}
