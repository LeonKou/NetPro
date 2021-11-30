using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NetPro
{
    /// <summary>
    /// 批量注入
    /// </summary>
    public static class BatchInjectionExtension
    {
        /// <summary>
        /// 批量注入
        /// </summary>
        /// <param name="services"></param>
        /// <param name="assemblyPattern">程序集正则表达式。this.GetType().Assembly.GetName().Name</param>
        /// <param name="classNamePattern">批量注入的类名正则表达式。Service$：以Service结尾的</param>
        public static void BatchInjection(this IServiceCollection services, string assemblyPattern, string classNamePattern = "Service$")
        {
            var typeFinder = services.BuildServiceProvider().GetRequiredService<ITypeFinder>();

            services.Scan(scan => scan
                .FromAssemblies(typeFinder.GetAssemblies().Where(s => s.GetName().Name.IsValidName(assemblyPattern)))
                .AddClasses(classes => classes.Where(type => type.Name.IsValidName(classNamePattern)))
                .AsImplementedInterfaces()
                .WithScopedLifetime());

            //template code
            //services.Scan(scan => scan
            //  .FromAssemblies(typeFinder.GetAssemblies().Where(s => s.GetName().Name.EndsWith("API")).ToArray())
            //  .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Service")))
            //  .AsImplementedInterfaces()
            //  .WithScopedLifetime());
        }

        private static bool IsValidName(this string currencyValue, string pattern)
        {
            return Regex.IsMatch(currencyValue, pattern);
        }
    }
}
