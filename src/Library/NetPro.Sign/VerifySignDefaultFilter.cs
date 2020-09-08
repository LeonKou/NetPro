using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetPro.Sign
{
    public class VerifySignDefaultFilter : IOperationFilter
    {
        private readonly IConfiguration _configuration;

        public VerifySignDefaultFilter(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// 获取游戏secret
        /// </summary>
        /// <param name="gameId"></param>
        /// <returns></returns>
        public virtual string GetSignSecret(string appid)
        {
            var secret = _configuration.GetValue<string>($"{nameof(VerifySignOption)}:AppSecret:AppId:{appid}");

            return secret;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="secret"></param>
        /// <param name="signMethod">签名算法；支持hmac-sha256；md5</param>
        /// <returns>签名16进制</returns>
        public virtual string GetSignhHash(string message, string secret, string signMethod)
        {
            switch (signMethod)
            {
                case "hmac-sha256":
                    return SignCommon.GetHMACSHA256Sign(message, secret);
                case "md5":
                    return SignCommon.CreateMD5(message, secret);
                default:
                    return SignCommon.GetHMACSHA256Sign(message, secret);
            }
            
        }

    }
}
