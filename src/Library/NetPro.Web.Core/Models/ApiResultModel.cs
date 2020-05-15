using Microsoft.AspNetCore.Mvc;

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
        public T Body { get; set; }
    }

    /// <summary>
    /// api 返回结果分装 空body
    /// </summary>
    public class ApiResultModel : ApiResultModel<string>
    {
        public ApiResultModel()
        {
            Body = string.Empty;
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

    /// <summary>
    /// Controller响应结果
    /// 再不继承ApiControllerBase的情况下可直接用此实体响应
    /// </summary>
    public static class ResponseResult
    {
        public static IActionResult ToFailResult(string msg, int errorCode = 100)
        {
            var resultModel = new ApiResultModel()
            {
                ErrorCode = errorCode,
                Msg = msg
            };
            var result = new ObjectResult(resultModel);
            result.StatusCode = errorCode;
            return result;
        }

        /// <summary>
        /// 成功返回结果.body为空
        /// </summary>
        /// <param name="msg">成功提示消息</param>
        /// <returns></returns>
        public static IActionResult ToSuccessResult(string msg = "")
        {
            var result = new ApiResultModel()
            {
                ErrorCode = 0,
                Msg = msg
            };
            return new JsonResult(result);
        }

        /// <summary>
        /// 成功返回结果.带body数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="body"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static IActionResult ToSuccessResult<T>(T body, string msg = "")
        {
            var result = new ApiResultModel<T>()
            {
                Body = body,
                ErrorCode = 0,
                Msg = msg
            };
            return new JsonResult(result);
        }
    }
}
