using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetPro.Pulsar
{
    /// <summary>
    /// pulsar配置
    /// </summary>
    public class PulsarOption
    {
        /// <summary>
        /// service URL
        /// </summary>
        public string? ServiceUrl { get; set; }

        /// <summary>
        /// 认证信息
        /// </summary>
        public AuthenticationInfo? Authentication { get; set; }
    }
}
