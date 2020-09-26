namespace NetPro.Core.Configuration
{
    /// <summary>
    /// token 认证参数
    /// </summary>
    public class JwtBearerConfig
    {
        /// <summary>
        /// 颁发给谁  
        /// </summary>
        public string ValidIssuer { get; set; }
        /// <summary>
        /// Token颁发机构  
        /// </summary>
        public string ValidAudience { get; set; }
        /// <summary>
        /// 秘钥
        /// </summary>
        public string Key { get; set; }
    }
}
