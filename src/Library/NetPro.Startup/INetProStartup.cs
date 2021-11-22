using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetPro.TypeFinder;

namespace NetPro
{
    /// <summary>
    /// 程序启动时配置、中间件服务 注册依赖 接口定义
    /// 继承此接口将在初始化启动时自动执行实现了ConfigureServices和Configure的方法
    /// </summary>
    public interface INetProStartup
    {
        /// <summary>
        /// 添加或自定义中间件服务(程序启用需要使用哪些中间件服务)
        /// </summary>
        /// <param name="services">服务描述符的集合</param>
        /// <param name="configuration">应用程序的配置器</param>
        /// <param name="typeFinder">类型发现器</param>
        void ConfigureServices(IServiceCollection services, IConfiguration configuration = null, ITypeFinder typeFinder = null);

        /// <summary>
        /// 请求管道的配置应用程序
        /// </summary>
        /// <param name="app">用于配置应用程序请求管道的构建器</param>
        void Configure(IApplicationBuilder app);

        /// <summary>
        /// 依赖注入调用顺序
        /// double.double.MaxValue系统内置，请勿使用
        /// </summary>
        double Order { get; set; }
    }
}
