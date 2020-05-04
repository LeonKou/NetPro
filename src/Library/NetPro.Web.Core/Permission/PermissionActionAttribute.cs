using System;

namespace NetPro.Web.Core.Permission
{
    /// <summary>
    /// action权限特征
    /// </summary>
    public class PermissionActionAttribute:Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ignore">是否忽略权限验证,登录用户都可以访问</param>
        /// <param name="programCode"></param>
        /// <param name="redirectUrl"></param>
        public PermissionActionAttribute(bool ignore,string programCode="",string redirectUrl = "")
        {
            this.Ignore = ignore;
            this.ProgramCode = programCode;
            this.RedirectUrl = redirectUrl;
        }

        /// <summary>
        /// 是否忽略权限验证,登录用户都可以访问
        /// </summary>
        public bool Ignore { get; set; }
        /// <summary>
        /// 页面操作代码
        /// </summary>
        public string ProgramCode { get; set; }

        /// <summary>
        /// 无权限跳转页面地址
        /// </summary>
        public string RedirectUrl { get; set; }
    }
}
