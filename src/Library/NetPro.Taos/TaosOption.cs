using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace NetPro.Taos
{
    /// <summary>
    /// TaosOption配置
    /// </summary>
    public class TaosOption
    {
        /// <summary>
        /// 
        /// </summary>
        public TaosOption()
        {
        }

        /// <summary>
        /// root node is Redis
        /// </summary>
        /// <param name="config"></param>
        public TaosOption(IConfiguration config)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            config.GetSection(nameof(TaosOption)).Bind(this);
        }

        /// <summary>
        /// 空闲时间
        /// 单位秒，默认120秒
        /// </summary>
        public int Idle { get; set; } = 120;

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
