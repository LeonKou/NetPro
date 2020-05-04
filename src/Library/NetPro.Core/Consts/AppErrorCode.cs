using NetPro.Core.Infrastructure.Attributes;

namespace NetPro.Core.Consts
{
    /// <summary>
    ///系统层级错误代码定义(与业务无关) 值在1-100之间
    /// </summary>
    public enum AppErrorCode
    {
        /// <summary>
        /// 未知
        /// </summary>
        [ErrorCodeLevel(NetProErrorLevel.Error)]
        None = 100,
        /// <summary>
        /// 程序发生异常
        /// </summary>
        [ErrorCodeLevel(NetProErrorLevel.Error)]
        Exception = 1,
        /// <summary>
        /// viewModel验证失败
        /// </summary>
        [ErrorCodeLevel(NetProErrorLevel.Warning)]
        ModelInValid = 2,
        /// <summary>
        /// 未授权访问
        /// </summary>
        [ErrorCodeLevel(NetProErrorLevel.Warning)]
        Unauthorization = 3,
        /// <summary>
        /// 权限不足
        /// </summary>
        [ErrorCodeLevel(NetProErrorLevel.Warning)]
        PermissionDenied = 4,
        /// <summary>
        /// 参数为空
        /// </summary>
        [ErrorCodeLevel(NetProErrorLevel.Error)]
        ArgumentEmpty = 5,
        /// <summary>
        /// 数据不存在
        /// </summary>
        [ErrorCodeLevel(NetProErrorLevel.Warning)]
        NotExist = 6,
        /// <summary>
        /// Http请求异常
        /// </summary>
        [ErrorCodeLevel(NetProErrorLevel.Error)]
        HttpRequest = 7,
        /// <summary>
        /// 签名验证失败
        /// </summary>
        [ErrorCodeLevel(NetProErrorLevel.Error)]
        VerifySignFailt = 8,
    }
}
