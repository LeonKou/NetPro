﻿using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetPro.TypeFinder;

namespace NetPro.Core.Infrastructure
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
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="configuration">Configuration root of the application</param>
        /// <param name="typeFinder"></param>
        void ConfigureServices(IServiceCollection services, IConfiguration configuration = null, ITypeFinder typeFinder = null);

        /// <summary>
        /// 使用中间件
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        void Configure(IApplicationBuilder application);

        /// <summary>
        /// 依赖注入调用顺序
        /// </summary>
        int Order { get; }

        /// <summary>
        /// 描述
        /// 方便在控制台直观显示继承了INetProStartup的中间件各用途
        /// </summary>
        string Description { get; }
    }
}
