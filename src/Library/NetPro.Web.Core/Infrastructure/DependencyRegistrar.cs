using System.Linq;
using NetPro.Core.Configuration;
using NetPro.Core.Consts;
using NetPro.Core.Infrastructure;
using NetPro.Core.Infrastructure.DependencyManagement;
using NetPro.Utility.Helpers;
using NetPro.Web.Core.Permission;
using Autofac;
using AutofacSerilogIntegration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using NetPro.Web.Core.Helpers;
using NetPro.TypeFinder;

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
        /// <param name="configuration"></param>
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder, NetProOption config)
        {
            //file provider
            //builder.RegisterType<NetProFileProvider>().As<INetProFileProvider>().InstancePerLifetimeScope();
            //builder.RegisterType<NetProFileProvider>().As<INetProFileProvider>().PropertiesAutowired();//属性注入；TODO 保留，可能用到，实际注入在NetPro.TypeFinder
            //web helper
            builder.RegisterType<WebHelper>().As<IWebHelper>().InstancePerLifetimeScope();
            builder.RegisterType<ActionContextAccessor>().As<IActionContextAccessor>().InstancePerLifetimeScope();

            builder.RegisterAssemblyTypes(typeFinder.GetAssemblies()
                .Where(r => RegexHelper.IsMatch(r.GetName().Name, $"^{config.ProjectPrefix}.*({config.ProjectSuffix}|Service)$")).ToArray())
             .Where(t => t.Name.EndsWith("Service"))
           .AsImplementedInterfaces().InstancePerLifetimeScope();
            //builder.RegisterAssemblyTypes(typeFinder.GetAssemblies().Where(r => RegexHelper.IsMatch(r.GetName().Name, $"^{config.ProjectPrefix}.*({config.ProjectSuffix}|Repository)$")).ToArray())
            //.Where(t => t.Name.EndsWith("Repository"))
            //.AsImplementedInterfaces().InstancePerLifetimeScope();
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
