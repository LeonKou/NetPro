using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace NetPro.Tdengine
{
    /// <summary>
    /// TdengineOption配置
    /// </summary>
    public class TdengineOption
    {
        /// <summary>
        /// 
        /// </summary>
        public TdengineOption()
        {
            ConnectionString = new();
        }

        /// <summary>
        ///TdengineOption配置
        /// </summary>
        /// <param name="config"></param>
        public TdengineOption(IConfiguration config)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }
            ConnectionString = new();

            config.GetSection(nameof(TdengineOption)).Bind(this);
        }

        /// <summary>
        /// 连接串集合
        /// </summary>
        public List<ConnectionString> ConnectionString { get; set; }

    }

    /// <summary>
    /// 
    /// </summary>
    public class ConnectionString
    {
        /// <summary>
        /// 连接串别名
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 连接串
        /// </summary>
        public string Value { get; set; }
    }
}
