using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace NetPro.Checker
{
    public class AppInfo
    {
        public Dictionary<string, string> RequestHeaders { get; set; }

        public List<string> ConfigProviders { get; } = new List<string>();

        public IEnumerable<KeyValuePair<string, string>> Configs { get; private set; }

        public static AppInfo GetAppInfo(IConfiguration configuration)
        {
            var info = new AppInfo();

            foreach (var provider in ((IConfigurationRoot)configuration).Providers.ToList())
            {
                info.ConfigProviders.Add(provider.ToString());
            }

            info.Configs = configuration.AsEnumerable();

            return info;
        }
    }
}