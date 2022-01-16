namespace NetPro.Core.Configuration
{
    /// <summary>
    /// Represents startup  configuration parameters
    /// </summary>
    public partial class NetProOption
    {
        /// <summary>
        ///请求头参数"Cache-Control" 
        /// </summary>
        public string StaticFilesCacheControl { get; set; } = "Cache-Control";

        /// <summary>
        /// http 返回内容是否压缩
        /// </summary>
        public bool UseResponseCompression { get; set; } = false;

        /// <summary>
        /// 最小线程数(cpu核心数2倍)
        /// </summary>
        public int ThreadMinCount { get; set; }

        /// <summary>
        /// 应用程序名称
        /// </summary>
        public string ApplicationName { get; set; }

        /// <summary>
        /// api 执行超时警告时长(秒)
        /// </summary>
        public int RequestWarningThreshold { get; set; } = 5;

        /// <summary>
        /// 404页面地址
        /// </summary>
        public string PageNotFoundUrl { get; set; }
        /// <summary>
        /// 是否是调试模式
        /// </summary>
        public bool IsDebug { get; set; }

        /// <summary>
        /// 全局路由前缀
        /// </summary>
        /// <remarks>例如：/api/admin/xxx/controller/action </remarks>
        public string RoutePrefix { get; set; }

    }
}