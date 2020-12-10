using Autofac;
using AutofacSerilogIntegration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using NetPro.Core.Configuration;
using NetPro.Core.Consts;
using NetPro.Core.Infrastructure.DependencyManagement;
using NetPro.TypeFinder;
using NetPro.Utility.Helpers;
using NetPro.Web.Core.Helpers;
using NetPro.Web.Core.Permission;
using System.Linq;

namespace NetPro.Web.Core.Infrastructure
{
    /// <summary>
    /// 应用程序依赖注册服务实现 注册缓存、文件、数据库、业务service服务
    /// </summary>
    public class DependencyRegistrar : IDependencyRegistrar
    {
        /// <summary>
        /// Register services and interfaces
        /// </summary>
        /// <param name="builder">Container builder</param>
        /// <param name="typeFinder">Type finder</param>
        /// <param name="config">Config</param>
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder, NetProOption config)
        {
            builder.RegisterType<WebHelper>().As<IWebHelper>().InstancePerLifetimeScope();
            builder.RegisterType<ActionContextAccessor>().As<IActionContextAccessor>().InstancePerLifetimeScope();

            var services = typeFinder.GetAssemblies()
              .Where(r => RegexHelper.IsMatch(r.GetName().Name, $"^{config.ProjectPrefix}.*({config.ProjectSuffix}|Service)$")).ToArray();
            builder.RegisterAssemblyTypes(services)
             .Where(t => t.Name.EndsWith("Service"))
           .AsImplementedInterfaces().InstancePerLifetimeScope();

            builder.RegisterAssemblyTypes(typeFinder.GetAssemblies().Where(r => RegexHelper.IsMatch(r.GetName().Name, $"^{config.ProjectPrefix}.*({config.ProjectSuffix}|Aggregate)$")).ToArray())
            .Where(t => t.Name.EndsWith("Aggregate"))
            .AsImplementedInterfaces().InstancePerLifetimeScope();

            builder.RegisterLogger(autowireProperties: true);

            var baseType = typeof(ControllerBase);
            builder.RegisterAssemblyTypes(typeFinder.GetAssemblies().ToArray())
                   .Where(t => baseType.IsAssignableFrom(t) && t != baseType).InstancePerLifetimeScope();

            if (config.AppType == AppType.Api)//api 没有权限验证判断使用默认的
            {
                builder.RegisterType<NullPermissionService>().As<INetProPermissionService>().InstancePerLifetimeScope();
            }
        }

        /// <summary>
        /// Gets order of this dependency registrar implementation
        /// </summary>
        public int Order
        {
            get { return -99; }
        }
    }
}
