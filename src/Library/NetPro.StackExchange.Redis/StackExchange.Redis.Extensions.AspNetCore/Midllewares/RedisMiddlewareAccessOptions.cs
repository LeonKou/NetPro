using Microsoft.AspNetCore.Http;
using System;
using System.Net;

namespace StackExchange.Redis.Extensions.AspNetCore.Midllewares
{
    /// <summary>
    /// All the options needed to allow the client to redis information middleware
    /// </summary>
    public class RedisMiddlewareAccessOptions
    {
        /// <summary>
        /// 获取或设置允许你自定义哪些ip可以访问redis信息的功能。
        /// </summary>
        /// <value>The funcion.</value>
        public Func<HttpContext, bool> AllowFunction { get; set; }

        /// <summary>
        /// 获取或设置允许显示redis服务器信息的IP。
        /// </summary>
        /// <value>An an array with the allowed ips</value>
        public IPAddress[] AllowedIPs { get; set; }
    }
}
