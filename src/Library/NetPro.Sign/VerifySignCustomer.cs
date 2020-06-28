﻿using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetPro.Sign
{
    public class VerifySignCustomer : IOperationFilter
    {
        private readonly IConfiguration _configuration;

        public VerifySignCustomer(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// 根据appid获取secret
        /// </summary>
        /// <param name="gameId"></param>
        /// <returns></returns>
        public string GetSignSecret(string appid)
        {
            var secret = "1111";
            return secret;
        }

        /// <summary>
        /// 定义摘要算法
        /// </summary>
        /// <param name="message"></param>
        /// <param name="secret"></param>
        /// <returns></returns>
        public string GetSignhHash(string message, string secret)
        {
            return "5555555";
        }
    }
}
