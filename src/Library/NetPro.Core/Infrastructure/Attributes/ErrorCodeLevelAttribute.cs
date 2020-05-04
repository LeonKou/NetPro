using NetPro.Core.Consts;
using System;

namespace NetPro.Core.Infrastructure.Attributes
{
    /// <summary>
    /// 错误代码级别特征
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class ErrorCodeLevelAttribute : Attribute
    {
        /// <summary>
        /// 日志级别
        /// </summary>
        public NetProErrorLevel Level { get; set; }

        public ErrorCodeLevelAttribute(NetProErrorLevel level)
        {
            Level = level;
        }
    }
}
