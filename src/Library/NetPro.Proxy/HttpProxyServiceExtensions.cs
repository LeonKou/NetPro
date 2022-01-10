using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.NetPro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Net.Http;

namespace NetPro.Proxy
{
    /// <summary>
    /// 
    /// </summary>
    public static class HttpProxyServiceExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <param name="typeFinder"></param>
        /// <param name="assemblyPattern ">匹配使用NetPro.Proxy远程请求功能的程序集名称的正则表达式</param>
        /// <param name="interfacePattern ">匹配使用NetPro.Proxy远程请求功能定义的接口名称的正则表达式</param>
        /// <returns></returns>
        public static IServiceCollection AddHttpProxy(this IServiceCollection services, IConfiguration configuration, ITypeFinder typeFinder, string assemblyPattern = null, string interfacePattern = null)
        {
            var assemblys = new List<Assembly>();
            if (string.IsNullOrWhiteSpace(assemblyPattern))
            {
                var assemblies = typeFinder.GetAssemblies();
                assemblys = assemblies.Where(r => IsMatch(r.GetName().Name, $"Proxy$")).ToList();
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss} HttpProxy(WebApiClient)远程请求程序集默认模式，程序集名称: {string.Join(';', assemblys.Select(s => s.GetName().Name))}");
            }
            else
            {
                var assemblies = typeFinder.GetAssemblies();
                assemblys = assemblies.Where(r => IsMatch(r.GetName().Name, assemblyPattern)).ToList();
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss} HttpProxy(WebApiClient))远程请求组件已经指定生效的程序集，程序集名称: {string.Join(';', assemblys.Select(s => s.GetName().Name))}");
            }

            var cookieContainer = new CookieContainer();
            if (string.IsNullOrWhiteSpace(interfacePattern))
            {
                interfacePattern = ".*.";
            }
            foreach (var assembly in assemblys)
            {
                if (assembly != null)
                {
                    //var typeConfigurations = type.GetTypes().Where(type =>
                    //     type.IsInterface).Where(t => t.Name.EndsWith("Proxy"));
                    var typeConfigurations = assembly.GetTypes().Where(type =>
                        type.IsInterface).Where(r => IsMatch(r.Name, interfacePattern));

                    foreach (var item in typeConfigurations)
                    {
                        var method = item.GetMethods()?.FirstOrDefault();
                        var proxyCharacteristic = method?.CustomAttributes?.FirstOrDefault()?.AttributeType?.BaseType;

                        if (proxyCharacteristic != typeof(WebApiClientCore.Attributes.HttpMethodAttribute))
                        {
                            continue;
                        }
                        services.AddHttpApi(item, s =>
                       {
                           //var servicename = GetValue(item.Name, "I(.*)Proxy", "$1");
                           var servicename = item.Name;
                           var host = configuration.GetValue<string>($"{nameof(NetProProxyOption)}:{servicename}");
                           if (string.IsNullOrWhiteSpace(host))
                           {
                               throw new ArgumentNullException($"{nameof(NetProProxyOption)}:{servicename}", $"未检测到服务终结点NetProProxyOption:{servicename},请检查配置文件中是否包含NetProProxyOption节点");
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
