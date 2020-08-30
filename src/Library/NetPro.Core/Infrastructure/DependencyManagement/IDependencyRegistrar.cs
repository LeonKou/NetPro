using Autofac;
using NetPro.Core.Configuration;
using NetPro.TypeFinder;

namespace NetPro.Core.Infrastructure.DependencyManagement
{
    /// <summary>
    /// 依赖注册接口定义 实现此接口可以使用autofac依赖注入
    /// </summary>
    public interface IDependencyRegistrar
    {
        /// <summary>
        /// 注册服务或接口
        /// </summary>
        /// <param name="builder">ContainerBuilder对象</param>
        /// <param name="typeFinder">类型查找器 </param>
        /// <param name="config">配置参数</param>
        void Register(ContainerBuilder builder, ITypeFinder typeFinder, NetProOption config);

        /// <summary>
        ///依赖注册执行顺序 从小到大执行
        /// </summary>
        int Order { get; }
    }
}
