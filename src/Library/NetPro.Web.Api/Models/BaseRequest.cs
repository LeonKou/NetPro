using System;

namespace NetPro.Web.Api.Models
{
    /// <summary>
    /// 查询请求基类
    /// </summary>
    public class BaseRequest
    {
        /// <summary>
        /// 请求标识
        /// </summary>
        public string RequestId { get; set; } = Guid.NewGuid().ToString();
    }
}
