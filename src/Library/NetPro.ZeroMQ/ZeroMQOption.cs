using Microsoft.Extensions.Configuration;
using System;

namespace NetPro.ZeroMQ
{
    /// <summary>
    /// ZeroMQOption配置
    /// </summary>
    public class ZeroMQOption
    {
        /// <summary>
        /// 
        /// </summary>
        public ZeroMQOption()
        {
        }

        /// <summary>
        /// ZeroMQOption
        /// </summary>
        /// <param name="config"></param>
        public ZeroMQOption(IConfiguration config)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            config.GetSection(nameof(ZeroMQOption)).Bind(this);
        }

        /// <summary>
        /// 发布订阅端口
        /// </summary>
        public int PublishPort { get; set; } = 81;

        /// <summary>
        /// 推拉端口
        /// </summary>
        public int PushPort { get; set; } = 82;

        /// <summary>
        /// 请求响应端口
        /// </summary>
        public int ResponsePort { get; set; } = 83;
    }
}
