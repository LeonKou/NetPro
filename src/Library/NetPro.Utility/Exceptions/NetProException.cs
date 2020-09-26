using System;
using System.Runtime.Serialization;

namespace NetPro.Utility
{
    /// <summary>
    /// 异常基类
    /// </summary>
    public class NetProException : Exception
    {
        /// <summary>
        /// 错误Code码
        /// </summary>
        public int ErrorCode { get; set; } = 1;
        /// <summary>
        /// 请求Id
        /// </summary>
        public string RequestId { get; set; }

        public NetProException()
        {

        }

        /// <summary>
        ///记录错误信息构造函数
        /// </summary>
        /// <param name="message">消息内容</param>
        /// <param name="errorCode">异常代码</param>
        /// <param name="requestId">请求Id</param>
        public NetProException(string message, int errorCode = 1, string requestId = "")
            : base(message)
        {
            this.ErrorCode = errorCode;
            this.RequestId = requestId;
        }

        /// <summary>
        /// 记录异常信息构造函数 string.format
        /// </summary>
		/// <param name="messageFormat">消息格式化内容</param>
		/// <param name="args">异常消息参数</param>
        public NetProException(string messageFormat, params object[] args)
            : base(string.Format(messageFormat, args))
        {
        }

        /// <summary>
        /// 记录异常信息构造函数 string.format
        /// </summary>
        /// <param name="errorCode">错误代码</param>
		/// <param name="messageFormat">消息格式化内容</param>
		/// <param name="args">异常消息参数</param>
        public NetProException(int errorCode, string messageFormat, params object[] args)
            : base(string.Format(messageFormat, args))
        {
            this.ErrorCode = errorCode;
        }

        /// <summary>
        /// 记录异常信息构造函数
        /// </summary>
        /// <param name="info">关于所引发异常的序列化对象数据</param>
        /// <param name="context">包含关于源或目标的上下文信息</param>
        protected NetProException(SerializationInfo
            info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// 记录异常信息构造函数 包含异常描述和异常对象
        /// </summary>
        /// <param name="message">异常消息描述.</param>
        /// <param name="innerException">异常对象</param>
        /// <param name="errorCode">异常代码</param>
        /// <param name="requestId">请求Id</param>
        public NetProException(string message, Exception innerException, int errorCode = 1, string requestId = "")
            : base(message, innerException)
        {
            this.ErrorCode = errorCode;
            this.RequestId = requestId;
        }
    }
}
