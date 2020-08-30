using NetPro.Core.Consts;
using NetPro.Utility;
using NetPro.Utility.Helpers;
using NetPro.Web.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetPro.Web.Core
{
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

        /// <summary>
        ///获取数据库连接字符串参数并解密
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="sectionName">只要最末级参数名称 ,必须放在NetPro:ConnectionStrings 节点下面</param>
        /// <param name="key">解密秘钥</param>
        /// <returns></returns>
        public static string GetDecryptConnection(this IConfiguration configuration, string sectionName, string key)
        {
            if (string.IsNullOrWhiteSpace(sectionName))
            {
                throw new NetProException("sectionName不能为空", AppErrorCode.ArgumentEmpty.Value());
            }
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new NetProException("key不能为空", AppErrorCode.ArgumentEmpty.Value());
            }
            sectionName = $"NetPro:ConnectionStrings:{sectionName}";
            string value = configuration.GetValue<string>(sectionName);
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new NetProException($"{sectionName}值不能为空", AppErrorCode.ArgumentEmpty.Value());
            }
            return EncryptHelper.DesDecrypt8(value, key);
        }
    }
}
