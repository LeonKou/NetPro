using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace NetPro.EasyNetQ
{
    /// <summary>
    /// mq配置
    /// </summary>
    public class EasyNetQOption
    {
        /// <summary>
        /// 
        /// </summary>
        public EasyNetQOption()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        public EasyNetQOption(IConfiguration config)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            config.GetSection(nameof(EasyNetQOption)).Bind(this);
        }

        /// <summary>
        /// 空闲时间
        /// 单位秒，默认60秒
        /// </summary>
        public int Idle { get; set; }

        /// <summary>
        /// 连接串集合
        /// </summary>
        public List<ConnectionString> ConnectionString { get; set; } = new List<ConnectionString>();

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
