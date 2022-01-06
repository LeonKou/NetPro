using Microsoft.AspNetCore.Mvc;

namespace System.NetPro
{
    /// <summary>
    /// 
    /// </summary>
    public static partial class Extensions
    {
        /// <summary>
        /// 返回错误的ActionResult
        /// </summary>
        /// <param name="errorMsg">错误信息</param>
        /// <param name="errorCode">错误代码</param>
        /// <returns></returns>
        public static JsonResult ToErrorActionResult(this string errorMsg, int errorCode)
        {
            var model = new ResponseResult()
            {
                Code = errorCode,
                Msg = errorMsg
            };
            return new JsonResult(model);
        }
    }
}
