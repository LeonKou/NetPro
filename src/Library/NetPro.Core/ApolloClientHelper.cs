using Com.Ctrip.Framework.Apollo;
using Com.Ctrip.Framework.Apollo.Enums;
using Com.Ctrip.Framework.Apollo.Internals;
using NetPro.Core.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Threading.Tasks;

namespace NetPro.Core.Helpers
{
	public static class ApolloClientHelper
	{
		public static void ApolloConfig(HostBuilderContext hostingContext, IConfigurationBuilder builder, string[] args)
		{
			var environmentName = hostingContext.HostingEnvironment.EnvironmentName;
			builder.SetBasePath(Directory.GetCurrentDirectory())
						.AddJsonFile("appsettings.json", true, true)
						.AddJsonFile($"appsettings.{environmentName}.json", true, true)
						.AddEnvironmentVariables()
						.AddCommandLine(args); // 添加环境变量

			var apolloSection = builder.Build().GetSection("Apollo");
			var apolloBuilder = builder.AddApollo(apolloSection).AddDefault();
			var apolloNamespaces = apolloSection.GetValue<string>("Namespaces");

			if (!string.IsNullOrWhiteSpace(apolloNamespaces))
			{
				var namespaces = apolloNamespaces.Split(',');
				for (var index = 0; index < namespaces.Length; index++)
				{
					if (!string.IsNullOrWhiteSpace(namespaces[index]))
					{
						apolloBuilder.AddNamespace(namespaces[index], GetNameSpaceType(namespaces[index]));
					}
				}
			}
		}

		private static ConfigFileFormat GetNameSpaceType(string nameSpace)
		{
			if (nameSpace.Contains("Json")) return ConfigFileFormat.Json;
			if (nameSpace.Contains("Xml")) return ConfigFileFormat.Xml;
			if (nameSpace.Contains("Yml")) return ConfigFileFormat.Yml;
			if (nameSpace.Contains("Yaml")) return ConfigFileFormat.Yaml;
			if (nameSpace.Contains("Txt")) return ConfigFileFormat.Txt;

			return ConfigFileFormat.Properties;
		}

		//public static void GetValuesByNamespace(this IConfiguration configuration, string namespaceName)
		//{
		//	var apollomanager = EngineContext.Current.Resolve<IConfigManager>();
		//	var config = apollomanager.GetConfig(namespaceName).GetAwaiter().GetResult();
		//	var names = config.GetPropertyNames();

		//}

	}
}
