using NetPro.Core.Helpers;
using NetPro.Core.Infrastructure;
using NetPro.Utility;
using NetPro.Utility.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using WebApiClient;

namespace NetPro.Web.Core.Infrastructure
{
	public class ApiProxyStartup : INetProStartup
	{
		public int Order => 900;

		public void Configure(IApplicationBuilder application)
		{

		}

		public void ConfigureServices(IServiceCollection services, IConfiguration configuration = null, ITypeFinder typeFinder = null)
		{
			var types = typeFinder.GetAssemblies().Where(r => RegexHelper.IsMatch(r.GetName().Name, "^NetPro.*\\.(Api|Proxy)$")).ToArray();

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
						});
					}
				}
			}
		}
	}
}
