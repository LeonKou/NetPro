using NetPro.Core.Consts;
using System;
using System.Collections.Generic;
using System.Text;

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
            this.Level = level;
        }
    }
}
