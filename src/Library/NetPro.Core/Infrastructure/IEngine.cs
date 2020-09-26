using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetPro.Core.Configuration;
using System;
using System.Collections.Generic;

namespace NetPro.Core.Infrastructure
{
    /// <summary>
    ///定义程序启动初始化配置、服务、模块、组件等注入
    /// </summary>
    public interface IEngine
    {
        /// <summary>
        /// Add and configure services
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="configuration">Configuration of the application</param>
        /// <param name="nopConfig">Nop configuration parameters</param>
        void ConfigureServices(IServiceCollection services, IConfiguration configuration, NetProOption nopConfig);

        /// <summary>
        /// Configure HTTP request pipeline
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        void ConfigureRequestPipeline(IApplicationBuilder application);

        /// <summary>
        /// Resolve dependency
        /// </summary>
        /// <typeparam name="T">Type of resolved service</typeparam>
        /// <returns>Resolved service</returns>
        T Resolve<T>() where T : class;

        /// <summary>
        /// Resolve dependency
        /// </summary>
        /// <param name="type">Type of resolved service</param>
        /// <returns>Resolved service</returns>
        object Resolve(Type type);

        /// <summary>
        /// Resolve dependencies
        /// </summary>
        /// <typeparam name="T">Type of resolved services</typeparam>
        /// <returns>Collection of resolved services</returns>
        IEnumerable<T> ResolveAll<T>();

        /// <summary>
        /// Resolve unregistered service
        /// </summary>
        /// <param name="type">Type of service</param>
        /// <returns>Resolved service</returns>
        object ResolveUnregistered(Type type);

        /// <summary>
        /// Register dependencies
        /// </summary>
        /// <param name="containerBuilder">Container builder</param>
        /// <param name="nopConfig">Nop configuration parameters</param>
        void RegisterDependencies(ContainerBuilder containerBuilder, NetProOption nopConfig);
    }
}
