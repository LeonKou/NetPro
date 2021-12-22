using HealthChecks.UI.Client;
using MediatR;
using NetPro.Checker;
using NetPro.TypeFinder;

namespace XXX.API
{
    /// <summary>
    /// 自定义启动类
    /// </summary>
    public class ApiStartup : INetProStartup
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
            //批量注入(可正则匹配注入所有子程序集，也可在每个子程序集中独立注入)
            services.BatchInjection("^XXX.", "Service$"); //批量注入以XXX前缀的程序集，Service结尾的类
            services.BatchInjection("^XXX.", "Repository$");//批量注入以XXX前缀的程序集，Repository结尾的类

            //https://ardalis.com/using-mediatr-in-aspnet-core-apps/
            services.AddMediatR(typeof(ApiStartup));
            //services.AddHealthChecks();
        }

        /// <summary>
        /// 请求管道配置
        /// </summary>
        /// <param name="application"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder application, IWebHostEnvironment env)
        {
            ////健康检查(应该限制外网访问或者注释此段)
            //application.UseHealthChecks("/health", new HealthCheckOptions()//健康检查服务地址
            //{
            //    Predicate = _ => true,
            //    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            //});

            ////环境检查(应该限制外网访问或者注释此段)
            //application.UseCheck(envPath: "/env", infoPath: "/info");//envPath:应用环境地址；infoPath:应用自身信息地址
        }
    }
}
