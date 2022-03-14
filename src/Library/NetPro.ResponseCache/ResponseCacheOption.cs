using System;
using System.Collections.Generic;

namespace NetPro.ResponseCache
{
    public class ResponseCacheOption
    {
        public ResponseCacheOption()
        {
            IgnoreVaryByQueryKeys = new List<string>();
        }
        /// <summary>
        /// 是否启用,默认启用
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// 
        /// </summary>
        public int Duration { get; set; }

        /// <summary>
        /// 全局忽略缓存的值
        /// </summary>
        public List<string> IgnoreVaryByQueryKeys { get; set; }

        /// <summary>
        /// 是否集群
        /// </summary>
        public bool Cluster { get; set; }
    }

    /// <summary>
    /// 忽略缓存
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = false)]
    public class IgnorePostResponseCacheAttribute : Attribute
    {

    }
}
