using System;
using System.Collections.Generic;
using System.Text;

namespace NetPro.Core.Configuration
{
    /// <summary>
    /// swaager文档描述配置
    /// </summary>
    public class SwaggerDocConfig
    {
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 是否开启SwaggerUI
        /// </summary>
        public bool EnableUI { get; set; } = false;
    }
}
