using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace NetPro.ZeroMQ
{
    /// <summary>
    /// TaosOption配置
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
        /// root node is Redis
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
    }
}
