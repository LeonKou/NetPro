﻿using Microsoft.AspNetCore.Http;
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
    public class VerifySignCommon: IOperationFilter
    {
        private readonly IConfiguration _configuration;

        public VerifySignCommon(IConfiguration configuration)
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
            var secret = _configuration.GetValue<string>($"VerifySignOption:AppSecret:AppId:{appid}");

            return secret;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="secret"></param>
        /// <returns>签名16进制</returns>
        public virtual string GetSignhHash(string message, string secret)
        {

            secret = secret ?? "";
            var encoding = new ASCIIEncoding();
            byte[] keyByte = encoding.GetBytes(secret);
            byte[] messageBytes = encoding.GetBytes(message);
            using (var hmacsha256 = new HMACSHA256(keyByte))
            {
                byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
                var hexString = hashmessage.Aggregate(new StringBuilder(),
                               (sb, v) => sb.Append(v.ToString("x2"))
                              ).ToString();
                return hexString;
            }
        }

        internal string ReadAsString(HttpRequest request)
        {
            try
            {
                if (request.ContentLength > 0)
                {
                    EnableRewind(request);
                    var encoding = GetRequestEncoding(request);
                    return ReadStream(request.Body, encoding);
                }
                return null;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        internal string ReadStream(Stream stream, Encoding encoding)
        {
            using (StreamReader sr = new StreamReader(stream, encoding, true, 1024, true))
            {
                var str = sr.ReadToEnd();
                stream.Seek(0, SeekOrigin.Begin);
                return str;
            }
        }

        internal Encoding GetRequestEncoding(HttpRequest request)
        {
            var requestContentType = request.ContentType;
            var requestMediaType = requestContentType == null ? default(MediaType) : new MediaType(requestContentType);
            var requestEncoding = requestMediaType.Encoding;
            if (requestEncoding == null)
            {
                requestEncoding = Encoding.UTF8;
            }
            return requestEncoding;
        }

        internal void EnableRewind(HttpRequest request)
        {
            if (!request.Body.CanSeek)
            {
                request.EnableBuffering();
                Task.WaitAll(request.Body.DrainAsync(CancellationToken.None));
            }
            request.Body.Seek(0L, SeekOrigin.Begin);
        }

        internal void BuildErrorJson(ActionExecutingContext context)
        {
            context.HttpContext.Response.StatusCode = 400;
            context.HttpContext.Response.ContentType = "application/json";
            context.Result = new BadRequestObjectResult(new { ErrorCode = -1, Message = $"签名验证失败" });
        }
    }
}
