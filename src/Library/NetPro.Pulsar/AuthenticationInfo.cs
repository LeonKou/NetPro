using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetPro.Pulsar
{
    /// <summary>
    /// 认证信息
    /// </summary>
    public class AuthenticationInfo
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string? UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string? Password { get; set; }

        /// <summary>
        /// 认证信息
        /// </summary>
        public string? Token { get; set; }
    }
}
