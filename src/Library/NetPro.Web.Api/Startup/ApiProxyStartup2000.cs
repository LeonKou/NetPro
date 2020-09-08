using NetPro.Core.Infrastructure;
using NetPro.Utility;
using NetPro.Utility.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using NetPro.Core.Configuration;
using NetPro.TypeFinder;
using System.Net.Http;
using System.Net;

namespace NetPro.Web.Api
{
	public class ApiProxyStartup : INetProStartup
	{
		public int Order => 2000;

		public void Configure(IApplicationBuilder application)
		{

		}

		public void ConfigureServices(IServiceCollection services, IConfiguration configuration = null, ITypeFinder typeFinder = null)
		{
			var netProOption = services.BuildServiceProvider().GetService<NetProOption>();
			var types = typeFinder.GetAssemblies().Where(r => RegexHelper.IsMatch(r.GetName().Name, $"^{netProOption.ProjectPrefix}.*({netProOption.ProjectSuffix}|Proxy)$")).ToArray();
			var cookieContainer = new CookieContainer();
			foreach (var type in types)
			{
				if (type != null)
				{
					var typeConfigurations = type.GetTypes().Where(type =>
						 type.IsInterface).Where(t => t.Name.EndsWith("Proxy"));
					foreach (var item in typeConfigurations)
					{
						services.AddHttpApi(item, s =>
						{
							var servicename = RegexHelper.GetValue(item.Name, "I(.*)Proxy", "$1");
							var host = configuration.GetValue<string>($"MicroServicesEndpoint:{servicename}");
							s.HttpHost = new Uri(host);
						}).ConfigurePrimaryHttpMessageHandler(() =>
						{
							var handler = new HttpClientHandler { UseCookies = true };//true相当于真实浏览器请求
							handler.CookieContainer = cookieContainer;
							return handler;
						});
					}
				}
			}
		}
	}
}
