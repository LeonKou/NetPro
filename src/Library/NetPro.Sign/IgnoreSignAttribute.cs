using System;

namespace NetPro.Sign
{
    /// <summary>
    /// 忽略签名
    /// </summary>
    [Obsolete("签名不建议特性方式使用，建议中间件方式使用，确保在过滤器之前执行")]
    public class IgnoreSignAttribute : Attribute
    {

    }
}
