using System.Collections.Generic;

namespace NetPro.Analysic
{
    /// <summary>
    /// 
    /// </summary>
    public class PolicyOption
    {
        /// <summary>
        /// 是否开启
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// 流量分析的接口地址
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// 最大成功次数
        /// </summary>
        public int MaxSucceedLimit { get; set; }

        /// <summary>
        /// 最大失败次数
        /// </summary>
        public int MaxErrorLimit { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class RequestAnalysisOption
    {
        /// <summary>
        /// 
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<PolicyOption> PolicyOption { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    internal class RedisKeys
    {
        /// <summary>
        ///  同ip一天内最大匿名接口请求成功次数
        /// </summary>
        /// <remarks>手机号对应的验证码</remarks>
        public static string Key_Request_Limit_IP_Succ = "Request:Limit_IP_Succ:";

        /// <summary>
        ///  同ip一天内最大匿名接口请求失败次数
        /// </summary>
        /// <remarks>手机号对应的验证码</remarks>
        public static string Key_Request_Limit_IP_Error = "Request:Limit_IP_Error:";
    }
}
