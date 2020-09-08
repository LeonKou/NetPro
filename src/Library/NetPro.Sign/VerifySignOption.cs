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

        /// <summary>
        /// 签名方案，global;attribute，默认中间件方式
        /// </summary>
        public string Scheme { get; set; }

        public int ExpireSeconds { get; set; } = 5;

        public DeclareCommonParameters CommonParameters { get; set; }

        public List<Type> OperationFilterDescriptors { get; set; } = new List<Type>();

        /// <summary>
        /// 忽略的路由不检查签名
        /// </summary>
        public string[] IgnoreRoute { get; set; }
    }

    public class DeclareCommonParameters
    {
        public string TimestampName { get; set; } = "timestamp";
        public string AppIdName { get; set; } = "appid";
        public string SignName { get; set; } = "sign";
    }

    public static class VerifySignOptionsExtensions
    {
        public static void OperationFilter<TFilter>(this VerifySignOption verifySignOption) where TFilter : IOperationFilter
        {
            verifySignOption.OperationFilterDescriptors.Add(typeof(TFilter));
        }
    }
}
