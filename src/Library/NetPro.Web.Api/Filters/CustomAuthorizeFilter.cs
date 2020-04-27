using NetPro.Core.Configuration;
using NetPro.Core.Consts;
using NetPro.Utility;
using NetPro.Web.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetPro.Web.Api.Filters
{
	/// <summary>
	/// 自定义认证过滤器 重写401 403返回值
	/// </summary>
	public class CustomAuthorizeFilter : IAsyncAuthorizationFilter
	{
		readonly NetProOption _config;
		readonly AuthorizationPolicy _policy;

		public CustomAuthorizeFilter(NetProOption config)
		{

		}

		public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
		{

		}
	}
}
