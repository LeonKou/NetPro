using Serilog;

namespace XXX.API
{
    /// <summary>
    /// 自定义启动类
    /// </summary>
    public class GrpcStartup : INetProStartup
    {
        /// <summary>
        /// 执行顺序
        /// </summary>
        public double Order { get; set; } = int.MaxValue;

        /// <summary>
        /// 服务注入
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <param name="typeFinder"></param>
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration = null, ITypeFinder typeFinder = null)
        {
            //如需serilog日志请取消以下serilog代码注释并安装响应nuget包
            Serilog.Log.Logger = new Serilog.LoggerConfiguration()
                                .ReadFrom.Configuration(configuration)
                                .CreateLogger();
        }

        /// <summary>
        /// 请求管道配置
        /// </summary>
        /// <param name="application"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder application, IWebHostEnvironment env)
        { 
        }
    }
}
