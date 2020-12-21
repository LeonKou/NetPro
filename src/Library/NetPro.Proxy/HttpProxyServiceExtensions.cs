using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetPro.TypeFinder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text.RegularExpressions;

namespace NetPro.Proxy
{
    public static class HttpProxyServiceExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <param name="typeFinder"></param>
        /// <param name="assemblyFullName">程序集完成名称</param>
        /// <returns></returns>
        public static IServiceCollection AddHttpProxy(this IServiceCollection services, IConfiguration configuration, ITypeFinder typeFinder, string assemblyFullName = null)
        {
            var types = new List<Assembly>();
            if (string.IsNullOrWhiteSpace(assemblyFullName))
            {
                types = typeFinder.GetAssemblies().Where(r => IsMatch(r.GetName().Name, $"Proxy$")).ToList();
            }
            else
            {
                types = typeFinder.GetAssemblies().Where(s => s.GetName().Name == assemblyFullName).ToList();
            }
            Console.WriteLine($"AddHttpProxy配置程序名为:{assemblyFullName}");

            var cookieContainer = new CookieContainer();
            foreach (var type in types)
            {
                if (type != null)
                {
                    var typeConfigurations = type.GetTypes().Where(type =>
                         type.IsInterface).Where(t => t.Name.EndsWith("Proxy"));
                    foreach (var item in typeConfigurations)
                    {
                        services.AddHttpApi(item, s =>
                        {
                            var servicename = GetValue(item.Name, "I(.*)Proxy", "$1");
                            var host = configuration.GetValue<string>($"MicroServicesEndpoint:{servicename}");
                            if (string.IsNullOrWhiteSpace(host))
                            {
                                throw new ArgumentNullException($"MicroServicesEndpoint:{servicename}", $"未检测到服务终结点MicroServicesEndpoint:{servicename},请检查配置文件中是否包含MicroServicesEndpoint节点");
                            }
                            s.HttpHost = new Uri(host);
                        }).ConfigurePrimaryHttpMessageHandler(() =>
                        {
                            var handler = new HttpClientHandler { UseCookies = true };//true相当于真实浏览器请求
                            handler.CookieContainer = cookieContainer;
                            return handler;
                        });
                    }
                }
            }
            return services;
        }

        /// <summary>
        /// 验证输入与模式是否匹配
        /// </summary>
        /// <param name="input">输入的字符串</param>
        /// <param name="pattern">模式字符串</param>
        /// <param name="options">选项</param>
        private static bool IsMatch(string input, string pattern, RegexOptions options = RegexOptions.IgnoreCase)
        {
            return Regex.IsMatch(input, pattern, options);
        }

        /// <summary>
        /// 获取匹配值
        /// </summary>
        /// <param name="input">输入字符串</param>
        /// <param name="pattern">模式字符串</param>
        /// <param name="resultPattern">结果模式字符串,范例："$1"用来获取第一个()内的值</param>
        /// <param name="options">选项</param>
        private static string GetValue(string input, string pattern, string resultPattern = "", RegexOptions options = RegexOptions.IgnoreCase)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;
            var match = Regex.Match(input, pattern, options);
            if (match.Success == false)
                return string.Empty;
            return string.IsNullOrWhiteSpace(resultPattern) ? match.Value : match.Result(resultPattern);
        }
    }
}
