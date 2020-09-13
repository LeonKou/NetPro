using System;
using System.Collections.Generic;
using System.Text;

namespace NetPro.ResponseCache
{
    public class ResponseCacheOption
    {
        /// <summary>
        /// 
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Duration { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<string> IgnoreVaryQuery { get; set; }
    }

    /// <summary>
    /// 忽略缓存
    /// </summary>
    [AttributeUsage(AttributeTargets.Method,Inherited = false)]
    public class IgnorePostResponseCacheAttribute : Attribute
    {

    }
}
