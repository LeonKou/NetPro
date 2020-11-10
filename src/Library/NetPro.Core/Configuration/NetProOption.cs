using NetPro.Core.Consts;

namespace NetPro.Core.Configuration
{
    /// <summary>
    /// Represents startup  configuration parameters
    /// </summary>
    public partial class NetProOption
    {
        /// <summary>
        /// 项目前缀，用于项目内部批量注入使用
        /// </summary>
        public string ProjectPrefix { get; set; } = "NetPro";

        /// <summary>
        /// 项目后缀，用于项目内部批量注入使用
        /// </summary>
        public string ProjectSuffix { get; set; }

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

        ///// <summary>
        ///// 是否启用性能监控
        ///// </summary>
        //public bool APMEnabled { get; set; }

        ///// <summary>
        ///// 是否打开权限过滤器(不同于认证)
        ///// </summary>
        //public bool PermissionEnabled { get; set; }

        ///// <summary>
        ///// 是否启用miniprofiler 监控
        ///// </summary>
        //public bool MiniProfilerEnabled { get; set; }

        /// <summary>
        /// 应用程序名称
        /// </summary>
        public string ApplicationName { get; set; }

        /// <summary>
        /// 超级角色 超级角色拥有系统所有权限
        /// </summary>
        public string SuperRole { get; set; } = "";
        /// <summary>
        /// api 执行超时警告时长(秒)
        /// </summary>
        public int RequestWarningThreshold { get; set; } = 5;

        /// <summary>
        /// 应用程序类型
        /// </summary>
        public AppType AppType { get; set; } = AppType.Api;

        /// <summary>
        /// 错误页面url
        /// </summary>
        public string ErrorUrl { get; set; }
        /// <summary>
        /// 权限认证方式 code,url
        /// </summary>
        public string Permission { get; set; } = "";
        /// <summary>
        /// 登录页面地址
        /// </summary>
        public string LoginUrl { get; set; }
        /// <summary>
        /// 404页面地址
        /// </summary>
        public string PageNotFoundUrl { get; set; }
        /// <summary>
        /// 是否是调试模式
        /// </summary>
        public bool IsDebug { get; set; }
        /// <summary>
        /// 跨域允许的站点
        /// </summary>
        public string CorsOrigins { get; set; } = "*";

        /// <summary>
        /// 是否启用健康检查
        /// </summary>
        public bool EnabledHealthCheck { get; set; }

    }
}