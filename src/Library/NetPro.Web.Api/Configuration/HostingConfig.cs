namespace NetPro.Web.Api
{
    /// <summary>
    /// Represents startup hosting configuration parameters
    /// </summary>
    public partial class HostingConfig
    {
        /// <summary>
        ///http请求头 X-Forwarded-For 的值
        /// </summary>
        public string ForwardedHttpHeader { get; set; }

        /// <summary>
        ///是否使用负载均衡
        /// </summary>
        public bool UseHttpClusterHttps { get; set; }

        /// <summary>
        /// 是否使用 HTTP_X_FORWARDED_PROTO
        /// </summary>
        public bool UseHttpXForwardedProto { get; set; }
    }
}