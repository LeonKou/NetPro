using NetPro.Web.Core.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using System.Linq;
using System.Text;

namespace NetPro.Web.Api
{
    // ReSharper disable once PartialTypeWithSinglePart
    public static partial class Extensions
    {
        /// <summary>
        /// model验证错误信息
        /// </summary>
        /// <param name="modelState"></param>
        /// <returns></returns>
        public static string ToModelStateError(this ModelStateDictionary modelState)
        {
            string split = "\n";
            StringBuilder sbError = new StringBuilder();
            (from key in modelState.Keys
             where modelState[key].Errors.Count > 0
             select key)
             .Aggregate(sbError, (sb, key) =>
             {
                 return modelState[key].Errors
         .Aggregate(sb, (sbChild, error) => sbChild.AppendFormat("{0}{1}", error.ErrorMessage, split));
             });
            string errorMsg = sbError.ToString();
            if (!string.IsNullOrEmpty(errorMsg) && errorMsg.EndsWith(split))
            {
                errorMsg = errorMsg.Substring(0, errorMsg.Length - split.Length);
            }
            return errorMsg;
        }

        /// <summary>
        /// model 请求model 字符串
        /// </summary>
        /// <param name="modelState"></param>
        /// <returns></returns>
        public static string ToBindModelText(this ModelStateDictionary modelState)
        {
            string split = "&";
            StringBuilder builder = new StringBuilder();
            (from key in modelState.Keys
             select key)
             .Aggregate(builder, (sb, key) =>
             {
                 return builder.AppendFormat("{0}={1}{2}", key, modelState[key].RawValue, split);
             });
            string bodyStr = builder.ToString();
            if (!string.IsNullOrEmpty(bodyStr) && bodyStr.EndsWith(split))
            {
                bodyStr = bodyStr.Substring(0, bodyStr.Length - split.Length);
            }
            return bodyStr;
        }

        /// <summary>
        /// 错误信息 封装成标准的json格式
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="errorCode"></param>
        /// <returns></returns>
        public static string ToErrorApiJson(this string errorMsg, int errorCode)
        {
            var model = new ApiResultModel()
            {
                ErrorCode = errorCode,
                Message = errorMsg
            };
            return JsonConvert.SerializeObject(model);
        }
    }
}
