using Microsoft.AspNetCore.Mvc;

namespace NetPro.Web.Core.Models
{
    /// <summary>
    /// 非Controller返回数据结构!!!
    /// </summary>
    /// <remarks>默认成功 Code=0;Msg=""</remarks>
    /// <typeparam name="T">泛型 Result返回类型</typeparam>
    public class ResponseResult<T>
    {
        /// <summary>
        /// 状态码， 0成功;-1 通用错误(xx不能为空;xx不能小于0等等);小于-1 致命错误; 大于0 提示性错误 
        /// 大于1000: 自定义业务提示码
        /// 等于1000:业务通用提示码
        /// 等于0：成功
        /// 等于-1:通用错误(xx不能为空;xx不能小于0等等)
        /// 等于-1000 系统性通用错误
        /// 小于-1000 自定义系统致命错误
        /// </summary>
        public int Code { get; set; } = 0;

        /// <summary>
        ///消息内容.失败原因、成功提示等.
        /// </summary>
        public string Msg { get; set; } = "";

        /// <summary>
        /// 数据内容                         
        /// </summary>
        public T Result { get; set; }
    }

    /// <summary>
    /// Controller返回专用结构!!!
    /// </summary>
    /// <remarks>默认成功 Code=0;Msg="",Result=null</remarks>
    /// <typeparam name="T">泛型 Result返回类型</typeparam>
    public class ResponseResult : ResponseResult<dynamic>
    {
        /// <summary>
        /// Controller返回专用结构
        /// </summary>
        public ResponseResult()
        {
            Result = null;
        }
    }
}
