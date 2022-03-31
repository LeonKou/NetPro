using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace NetPro.MongoDb
{
    /// <summary>
    /// TaosOption配置
    /// </summary>
    public class MongoDbOption
    {
        /// <summary>
        /// 
        /// </summary>
        public MongoDbOption()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        public MongoDbOption(IConfiguration config)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            config.GetSection(nameof(MongoDbOption)).Bind(this);
        }

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
        public string? Key { get; set; }

        /// <summary>
        /// 连接串
        /// </summary>
        public string? Value { get; set; }
    }
}
