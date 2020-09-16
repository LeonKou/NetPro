using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetPro.Sign
{
    public class VerifySignOption
    {
        public VerifySignOption()
        {

        }

        public bool IsDebug { get; set; }

        public bool IsForce { get; set; }

        public bool Enable { get; set; }

        public int ExpireSeconds { get; set; } = 5;

        public DeclareCommonParameters CommonParameters { get; set; }

        public List<Type> OperationFilterDescriptors { get; set; } = new List<Type>();
    }

    public class DeclareCommonParameters
    {
        public string TimestampName { get; set; } = "timestamp";
        public string AppIdName { get; set; } = "appid";
        public string SignName { get; set; } = "sign";

        public string EncryptFlag { get; set; } = "encryptflag";
    }

    public static class VerifySignOptionsExtensions
    {
        public static void OperationFilter<TFilter>(this VerifySignOption verifySignOption) where TFilter : IOperationFilter
        {
            verifySignOption.OperationFilterDescriptors.Add(typeof(TFilter));
        }
    }

    /// <summary>
    /// 签名加密
    /// </summary>
    [Flags]
    public enum EncryptEnum
    {
        /// <summary>
        /// Dafault 默认（签名默认HMACSHA256；脱敏默认不加密）
        /// </summary>
        Default = 0,

        /// <summary>
        /// 签名SHA256算法
        /// <remarks></remarks>
        /// </summary>
        SignSHA256 = 1,

        /// <summary>
        /// 签名MD5算法
        /// <remarks>1<<1</remarks>
        /// </summary>
        SignMD5 = 2,

        /// <summary>
        /// 脱敏AES
        /// <remarks></remarks>
        /// </summary>
        /// <remarks>1<<2</remarks>
        SymmetricAES = 4,

        /// <summary>
        /// 脱敏DES
        /// <remarks>1<<3</remarks>
        /// </summary>
        SymmetricDES = 8,

        /// <summary>
        /// 脱敏Base64
        /// <remarks>1<<4</remarks>
        /// </summary>
        /// <remarks>2^2;2^3</remarks>
        SymmetricBase64 = 16,

        /// <summary>
        /// 签名HMACSHA256算法
        /// <remark>1<<5</remark>
        /// </summary>
        SignHMACSHA256 = 32,
    }
}
