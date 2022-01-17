using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NetPro.Core.Consts;
using System.Linq;

namespace System.NetPro
{
    /// <summary>
    /// api 基类
    /// </summary>
    [ApiController] //模型验证失败，会在全局过滤器前返回，故无法通过全局过滤器统一返回，借助ConfigureApiBehaviorOptions可解决
    public abstract class ApiControllerBase : ControllerBase
    {
        private readonly ILogger<ApiControllerBase> _logger;
        /// <summary>
        /// 
        /// </summary>
        public ApiControllerBase()
        {
            var logger = EngineContext.Current.Resolve<ILogger<ApiControllerBase>>();
            _logger = logger;
        }

        #region api返回结果封装
        /// <summary>
        /// 错误返回
        /// </summary>
        /// <param name="msg">错误消息</param>
        /// <param name="errorCode">http错误码</param>
        /// <returns></returns>
        protected IActionResult ToFailResult(string msg, int errorCode = -1)
        {
            var resultModel = new ResponseResult()
            {
                Code = errorCode,
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
        protected virtual IActionResult ToSuccessResult(string msg = "")
        {
            var result = new ResponseResult()
            {
                Code = 0,
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
        protected virtual IActionResult ToSuccessResult<T>(T body, string msg = "")
        {
            var result = new ResponseResult<T>()
            {
                Result = body,
                Code = 0,
                Msg = msg
            };
            return new JsonResult(result);
        }
        #endregion

        /// <summary>
        /// 获取发起请求的系统平台
        /// </summary>
        /// <returns></returns>
        [NonAction]
        protected EnumAppPlatform GetPattern()
        {
            try
            {
                var result = Request.Headers["FromApp"].FirstOrDefault();

                var app = result;

                if (string.IsNullOrEmpty(app))
                    return EnumAppPlatform.None;

                var pattern = EnumAppPlatform.None;

                switch (app.ToLower())
                {
                    case "android":
                        pattern = EnumAppPlatform.Android;
                        break;
                    case "ios":
                        pattern = EnumAppPlatform.Ios;
                        break;
                    case "windows":
                        pattern = EnumAppPlatform.Windows;
                        break;
                    case "Web":
                        pattern = EnumAppPlatform.Web;
                        break;
                }
                return pattern;
            }
            catch (Exception ex)
            {
                if (_logger != null)
                {
                    _logger.LogError("BaseApiController.GetPattern", ex);
                }
                else
                {
                    Console.WriteLine($"ILogger<ApiControllerBase> is null;{ex.Message}");
                }
            }
            return EnumAppPlatform.None;
        }

        /// <summary>
        /// 获取客户端访问IP
        /// </summary>
        /// <returns></returns>
        [NonAction]
        protected string GetIP()
        {
            return EngineContext.Current.Resolve<IWebHelper>().GetCurrentIpAddress();
        }
    }
}



