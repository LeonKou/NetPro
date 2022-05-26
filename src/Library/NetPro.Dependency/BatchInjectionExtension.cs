/*
 *  MIT License
 *  
 *  Copyright (c) 2021 LeonKou
 *  
 *  Permission is hereby granted, free of charge, to any person obtaining a copy
 *  of this software and associated documentation files (the "Software"), to deal
 *  in the Software without restriction, including without limitation the rights
 *  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 *  copies of the Software, and to permit persons to whom the Software is
 *  furnished to do so, subject to the following conditions:
 *  
 *  The above copyright notice and this permission notice shall be included in all
 *  copies or substantial portions of the Software.
 *  
 *  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 *  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 *  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 *  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 *  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 *  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 *  SOFTWARE.
 */

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Linq;
using System.NetPro;
using System.Text.RegularExpressions;

/// <summary>
/// 批量注入
/// </summary>
public static class BatchInjectionExtension
{
    /// <summary>
    /// 批量注入
    /// </summary>
    /// <param name="services"></param>
    /// <param name="assemblyPattern">
    /// 程序集正则表达式
    /// 当前程序集名称：this.GetType().Assembly.GetName().Name</param>
    /// <param name="classNamePattern">批量注入的类名正则表达式</param>
    /// <remarks>
    /// 示例：
    /// Service$：匹配Service结尾的字符串;
    /// ^XXX.:匹配XXX.开头的字符串;
    /// *.XXX.*:匹配包含.XXX.的字符串;
    /// </remarks>
    public static IServiceCollection BatchInjection(this IServiceCollection services, string assemblyPattern, string classNamePattern = "Service$")
    {
        var typeFinder = services.BuildServiceProvider().GetService<ITypeFinder>();

        services.Scan(scan => scan
            .FromAssemblies(typeFinder.GetAssemblies().Where(s => s.GetName().Name.IsValidName(assemblyPattern)))
            .AddClasses(classes =>
            classes.Where(type =>
            {
                if (typeof(IHostedService).IsAssignableFrom(type))
                {
                    return false;
                }
                var succeed = type.Name.IsValidName(classNamePattern) && type.IsClass && !type.IsAbstract;
                return succeed;
            }
            ))
            .AsImplementedInterfaces()
            .WithScopedLifetime());
        return services;
        //template code
        //services.Scan(scan => scan
        //  .FromAssemblies(typeFinder.GetAssemblies().Where(s => s.GetName().Name.EndsWith("API")).ToArray())
        //  .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Service")))
        //  .AsImplementedInterfaces()
        //  .WithScopedLifetime());
    }

    /// <summary>
    /// 接口注入
    /// </summary>
    /// <param name="services"></param>
    public static void InterfaceDependency(this IServiceCollection services)
    {
        var typeFinder = services.BuildServiceProvider().GetService<ITypeFinder>();

        services.Scan(scan => scan
    .FromAssemblyOf<ITransientDependency>()
    .FromAssemblies(typeFinder.GetAssemblies())
    .AddClasses(classes => classes.AssignableTo<ITransientDependency>())
    .AsImplementedInterfaces()
    .WithTransientLifetime());

        services.Scan(scan => scan
     .FromAssemblyOf<ISingletonDependency>()
     .FromAssemblies(typeFinder.GetAssemblies())
     .AddClasses(classes => classes.AssignableTo<ISingletonDependency>())
     .AsImplementedInterfaces()
     .WithSingletonLifetime());

        services.Scan(scan => scan
        //.FromAssemblyOf<IScopedDependency>()
        .FromAssemblies(typeFinder.GetAssemblies())
        .AddClasses(classes => classes.AssignableTo<IScopedDependency>())
        .AsImplementedInterfaces()
           .WithScopedLifetime());
    }

    private static bool IsValidName(this string currencyValue, string pattern)
    {
        return Regex.IsMatch(currencyValue, pattern);
    }
}
