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
        /// 健康检车接口
        /// 默认HealthCheck
        /// </summary>
        public string HealthPath { get; set; } = "/HealthCheck";

        /// <summary>
        /// 
        /// </summary>
        public string[] Tags { get; set; }//= new string[] { "" };

        /// <summary>
        /// 服务名称
        /// 默认当前运行程序集名称
        /// </summary>
        public string ServiceName { get; set; } = Assembly.GetEntryAssembly().GetName().Name;

        /// <summary>
        /// 当前服务所在地址host:port
        /// 如："127.0.0.1:5001"
        /// </summary>
        //public string SelfAddress { get; set; }//当前服务所在地址只接受外部传入

        /// <summary>
        /// consul服务所在地址
        /// 格式：http://localhost:8500
        /// </summary>
        public string EndPoint { get; set; } = "http://localhost:8500";
    }
}
