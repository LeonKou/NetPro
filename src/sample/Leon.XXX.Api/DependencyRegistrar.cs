using Autofac;
using NetPro.Core.Configuration;
using NetPro.Core.Infrastructure.DependencyManagement;
using NetPro.TypeFinder;

namespace Leon.XXX.Api
{
    /// <summary>
    /// 应用程序依赖注册服务实现 
    /// </summary>
    public class DependencyRegistrar : IDependencyRegistrar
    {

        /// <summary>
        /// Gets order of this dependency registrar implementation
        /// </summary>
        public int Order
        {
            get { return 2; }
        }

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, NetProOption config)
        {
            //autofac方式注入dapper
            //builder.Register(c => new DefaultDapperContext(config.ConnectionStrings.ServerIdConnection["1"], DataProvider.Mysql)).InstancePerLifetimeScope();
        }
    }
}
