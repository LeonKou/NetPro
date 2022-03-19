using System;
using System.Collections.Generic;
using System.Reflection;

namespace NetPro.ConsulClient
{
    /// <summary>
    /// 服务治理第三方组件Consul相关配置参数
    /// </summary>
    public class ConsulOption
    {
        /// <summary>
        /// 是否开启，默认开启
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Tags
        /// </summary>
        public string[] Tags { get; set; }//= new string[] { "" };

        /// <summary>
        /// Meta
        /// </summary>
        public IDictionary<string, string> Meta { get; set; }

        /// <summary>
        /// 服务名称
        /// 默认当前运行程序集名称
        /// </summary>
        public string ServiceName { get; set; } = Assembly.GetEntryAssembly().GetName().Name;//AppDomain.CurrentDomain.FriendlyName;

        /// <summary>
        /// consul服务所在地址
        /// 格式：http://localhost:8500
        /// </summary>
        public string EndPoint { get; set; } = "http://localhost:8500";

        /// <summary>
        /// The default 30 seconds
        /// </summary>
        public TimeSpan? WaitTime { get; set; }// = TimeSpan.FromSeconds(5);

        /// <summary>
        /// 数据中心，默认空
        /// </summary>
        public string Datacenter { get; set; } = null;

        /// <summary>
        /// Token默认空
        /// </summary>
        public string Token { get; set; } = null;
    }
}
