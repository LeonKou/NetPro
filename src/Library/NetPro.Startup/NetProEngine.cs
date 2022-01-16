using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace System.NetPro
{
    /// <summary>
    /// IEngine接口实现
    /// </summary>
    public class NetProEngine : IEngine
    {
        #region Fields

        private ITypeFinder _typeFinder;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets service provider
        /// </summary>
        private IServiceProvider _serviceProvider { get; set; }

        #endregion

        #region Utilities

        /// <summary>
        /// Get IServiceProvider
        /// </summary>
        /// <returns>IServiceProvider</returns>
        protected IServiceProvider GetServiceProvider()
        {
            if (ServiceProvider == null)
                return null;
            var accessor = ServiceProvider?.GetService<IHttpContextAccessor>();
            return accessor?.HttpContext?.RequestServices ?? ServiceProvider;
        }

        ///// <summary>
        ///// Run startup tasks
        ///// </summary>
        ///// <param name="typeFinder">Type finder</param>
        //protected virtual void RunStartupTasks(ITypeFinder typeFinder)
        //{
        //    //find startup tasks provided by other assemblies
        //    var startupTasks = typeFinder.FindClassesOfType<IStartupTask>();

        //    //create and sort instances of startup tasks
        //    //we startup this interface even for not installed plugins. 
        //    //otherwise, DbContext initializers won't run and a plugin installation won't work
        //    var instances = startupTasks
        //        .Select(startupTask => (IStartupTask)Activator.CreateInstance(startupTask))
        //        .OrderBy(startupTask => startupTask.Order);

        //    //execute tasks
        //    foreach (var task in instances)
        //        task.Execute();
        //}

        ///// <summary>
        ///// Register dependencies
        ///// </summary>
        ///// <param name="containerBuilder">Container builder</param>
        ///// <param name="netProOption">configuration parameters</param>
        //public virtual void RegisterDependencies(ContainerBuilder containerBuilder, NetProOption netProOption)
        //{
        //    //https://blog.csdn.net/weixin_30679823/article/details/101501507
        //    //register engine
        //    containerBuilder.RegisterInstance(this).As<IEngine>().SingleInstance();

        //    //register type finder
        //    //containerBuilder.RegisterInstance(_typeFinder).As<ITypeFinder>().SingleInstance();

        //    //find dependency registrars provided by other assemblies
        //    var dependencyRegistrars = _typeFinder.FindClassesOfType<IDependencyRegistrar>();

        //    //create and sort instances of dependency registrars
        //    var instances = dependencyRegistrars
        //        .Select(dependencyRegistrar => (IDependencyRegistrar)Activator.CreateInstance(dependencyRegistrar))
        //        .OrderBy(dependencyRegistrar => dependencyRegistrar.Order);

        //    //register all provided dependencies
        //    foreach (var dependencyRegistrar in instances)
        //        dependencyRegistrar.Register(containerBuilder, _typeFinder);
        //}

        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            //check for assembly already loaded
            var assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.FullName == args.Name);
            if (assembly != null)
                return assembly;

            //get assembly from TypeFinder
            var tf = Resolve<ITypeFinder>();
            if (tf == null)
                return null;
            assembly = tf.GetAssemblies().FirstOrDefault(a => a.FullName == args.Name);
            return assembly;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Add and configure services
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        public void ConfigureServices(IServiceCollection services)
        {
            _serviceProvider = services.BuildServiceProvider(); //TODO 预留，运行成功前需要用到基础对象时打开

            if (_typeFinder == null)
                _typeFinder = services.BuildServiceProvider().GetRequiredService<ITypeFinder>();
            //services.AddSingleton<IEngine, NetProEngine>();
        }

        /// <summary>
        /// Configure HTTP request pipeline
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public void ConfigureRequestPipeline(IApplicationBuilder application)
        {
        }

        /// <summary>
        /// Resolve dependency
        /// </summary>
        /// <typeparam name="T">Type of resolved service</typeparam>
        /// <returns>Resolved service</returns>
        public T Resolve<T>() where T : class
        {
            return (T)Resolve(typeof(T));
        }

        /// <summary>
        /// Resolve dependency
        /// </summary>
        /// <param name="type">Type of resolved service</param>
        /// <returns>Resolved service</returns>
        public object Resolve(Type type)
        {
            var sp = GetServiceProvider();
            if (sp == null)
                return null;
            return sp.GetService(type);
        }

        /// <summary>
        /// Resolve dependencies
        /// </summary>
        /// <typeparam name="T">Type of resolved services</typeparam>
        /// <returns>Collection of resolved services</returns>
        public virtual IEnumerable<T> ResolveAll<T>()
        {
            return (IEnumerable<T>)GetServiceProvider().GetServices(typeof(T));
        }

        /// <summary>
        /// Resolve unregistered service
        /// </summary>
        /// <param name="type">Type of service</param>
        /// <returns>Resolved service</returns>
        public virtual object ResolveUnregistered(Type type)
        {
            Exception innerException = null;
            foreach (var constructor in type.GetConstructors())
            {
                try
                {
                    //try to resolve constructor parameters
                    var parameters = constructor.GetParameters().Select(parameter =>
                    {
                        var service = Resolve(parameter.ParameterType);
                        if (service == null)
                            throw new Exception("ResolveUnregistered() is exception ;Unknown dependency");
                        return service;
                    });

                    //all is ok, so create instance
                    return Activator.CreateInstance(type, parameters.ToArray());
                }
                catch (Exception ex)
                {
                    innerException = ex;
                }
            }

            throw new Exception("No constructor was found that had all the dependencies satisfied.", innerException);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Service provider
        /// </summary>
        public virtual IServiceProvider ServiceProvider => _serviceProvider;

        #endregion
    }
}
