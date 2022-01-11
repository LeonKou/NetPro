using Microsoft.Extensions.Configuration;
using System;
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

    /// <summary>
    /// 
    /// </summary>
    public class NetProCheckerOption
    {
        /// <summary>
        /// 是否开启
        /// </summary>
        public bool Enabled { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public NetProCheckerOption()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        public NetProCheckerOption(IConfiguration config)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            config.GetSection(nameof(NetProCheckerOption)).Bind(this);
        }

        /// <summary>
        /// EnvPath
        /// </summary>
        public string EnvPath { get; set; }
        /// <summary>
        /// InfoPath
        /// </summary>
        public string InfoPath { get; set; }
        /// <summary>
        /// HealthPath
        /// </summary>
        public string HealthPath { get; set; }
    }
}