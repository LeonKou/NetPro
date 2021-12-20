using Com.Ctrip.Framework.Apollo;
using Com.Ctrip.Framework.Apollo.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.IO;

namespace NetPro
{
    /// <summary>
    /// 
    /// </summary>
    public static class ApolloClientHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hostingContext"></param>
        /// <param name="builder"></param>
        /// <param name="args"></param>
        public static void ApolloConfig(HostBuilderContext hostingContext, IConfigurationBuilder builder, string[] args)
        {
            //TODO 递归加载指定文件夹下所有json文件
            var environmentName = hostingContext.HostingEnvironment.EnvironmentName;
            builder.SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", true, true)
                        .AddJsonFile($"appsettings.{environmentName}.json", true, true)
                        .AddEnvironmentVariables()
                        .AddCommandLine(args); // 添加环境变量

            if (!builder.Build().GetValue<bool>("Apollo:Enabled", false)) return;

            var apolloSection = builder.Build().GetSection("Apollo");
            var apolloBuilder = builder.AddApollo(apolloSection);
            //local config
            var appsettingNamespace = apolloSection.GetValue<string>("Namespaces");
            apolloBuilder.AddNamespace(appsettingNamespace);
            apolloBuilder.AddDefault();

            //remote config
            var apolloNamespaces = apolloBuilder.Build().GetValue<string>("Namespaces");

            if (string.IsNullOrWhiteSpace(apolloNamespaces)) return;
            var namespaces = apolloNamespaces.Split(',');
            foreach (var t in namespaces)
            {
                if (!string.IsNullOrWhiteSpace(t))
                {
                    apolloBuilder.AddNamespace(t, GetNameSpaceType(t));
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
    }
}
