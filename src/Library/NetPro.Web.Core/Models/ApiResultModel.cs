using System;
using System.Collections.Generic;
using System.Text;

namespace NetPro.Web.Core.Models
{
    /// <summary>
    /// api 返回结果分装 带body
    /// </summary>
    /// <typeparam name="T">泛型 body返回类型</typeparam>
    public class ApiResultModel<T> : BaseApiResultModel
    {         
        /// <summary>
        /// 数据内容
        /// </summary>
        public T Body { get; set; } = default(T);
    }

    /// <summary>
    /// api 返回结果分装 空body
    /// </summary>
    public class ApiResultModel : ApiResultModel<string>
    {
        public ApiResultModel()
        {
            this.Body = string.Empty;
        }
    }

    public class BaseApiResultModel
    {
        /// <summary>
        /// 错误代码 0成功，其它失败
        /// </summary>
        public int ErrorCode { get; set; }
        /// <summary>
        ///消息内容.失败原因、成功提示等.
        /// </summary>
        public string Msg { get; set; }
    }
}
