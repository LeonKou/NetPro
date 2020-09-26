namespace NetPro.Core.Configuration
{
    /// <summary>
    /// consul服务注册配置
    /// </summary>
    public class ConsulDiscoveryConfig
    {
        /// <summary>
        /// 服务名称
        /// </summary>
        public string ServiceName { get; set; }

        /// <summary>
        /// consul节点信息
        /// </summary>
        public ConsulOptions Consul { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ConsulOptions
    {
        /// <summary>
        ///consul服务终结点信息
        /// 格式: http://Host:Port
        /// </summary>
        public string HttpEndPoint { get; set; }
    }
}
