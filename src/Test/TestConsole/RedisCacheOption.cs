using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace RedisManager
{
    /// <summary>
    /// Redis配置
    /// </summary>
    public class RedisCacheOption : BaseRedisOptions
    {
        public RedisCacheOption()
        {
        }

        /// <summary>
        /// root node is nameof(RedisCacheOption) 
        /// </summary>
        /// <param name="config"></param>
        public RedisCacheOption(IConfiguration config)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            config.GetSection(nameof(RedisCacheOption)).Bind(this);
        }

        /// <summary>
        /// 
        /// </summary>
        public RedisCacheComponentEnum RedisComponent { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// The database.
        /// </value>
        public int Database { get; set; } = 0;

        /// <summary>
        /// 默认Key前缀
        /// 规范以:结尾
        /// :结尾可自动分组
        /// </summary>
        public string DefaultCustomKey { get; set; }

        /// <summary>
        /// 线程池数量
        /// </summary>
        public int PoolSize { get; set; } = 50;
    }

    public class BaseRedisOptions
    {
        /// <summary>
        /// Gets or sets the password to be used to connect to the Redis server.
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        public string Password { get; set; } = null;

        /// <summary>
        /// Gets or sets a value indicating whether to use SSL encryption.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is SSL; otherwise, <c>false</c>.
        /// </value>
        public bool IsSsl { get; set; } = false;

        /// <summary>
        /// Gets or sets the SSL Host.
        /// If set, it will enforce this particular host on the server's certificate.
        /// </summary>
        /// <value>
        /// The SSL host.
        /// </value>
        public string SslHost { get; set; } = null;

        /// <summary>
        /// Gets or sets the timeout for any connect operations.
        /// </summary>
        /// <value>
        /// The connection timeout.
        /// </value>
        public int ConnectionTimeout { get; set; } = 5000;

        /// <summary>
        /// Gets the list of endpoints to be used to connect to the Redis server.
        /// </summary>
        /// <value>
        /// The endpoints.
        /// </value>
        public List<ServerEndPoint> Endpoints { get; } = new List<ServerEndPoint>();
    }

    /// <summary>
    /// Defines an endpoint.
    /// </summary>
    public class ServerEndPoint
    {
        /// <summary>
        /// Gets or sets the port.
        /// </summary>
        /// <value>The port.</value>
        public int Port { get; set; }

        /// <summary>
        /// Gets or sets the host.
        /// </summary>
        /// <value>The host.</value>
        public string Host { get; set; }
    }

    /// <summary>
    /// Redis缓存组件类型
    /// </summary>
    public enum RedisCacheComponentEnum
    {
        /// <summary>
        /// 
        /// </summary>
        NullRedis = -1,

        /// <summary>
        /// 
        /// </summary>
        CSRedisCore = 1,

        /// <summary>
        /// 
        /// </summary>
        StackExchangeRedis = 2,
    }
}
