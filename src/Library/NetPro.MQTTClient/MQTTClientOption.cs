using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace NetPro.MQTTClient
{
    /// <summary>
    /// MQTT配置
    /// </summary>
    public class MQTTClientOption
    {
        /// <summary>
        /// 
        /// </summary>
        public MQTTClientOption()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        public MQTTClientOption(IConfiguration config)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            config.GetSection(nameof(MQTTClientOption)).Bind(this);

        }

        /// <summary>
        /// 连接串集合
        /// </summary>
        public List<ConnectionString> ConnectionString { get; set; }

        /// <summary>
        ///是否启用
        ///true:启用，false:禁用
        /// </summary>
        public bool Enabled { get; set; }
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
        /// 格式：
        /// clientid=netpro;host=127.0.0.1;port=6379;username=netpro;password=netpro;
        /// clientid:客户端id唯一标识
        /// host:broker主机地址
        /// port:broker主机端口
        /// username:账户名
        /// password:密码
        /// </summary>
        public string Value { get; set; }
    }
}
