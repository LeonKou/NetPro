using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NetPro.Core.Configuration;
using NetPro.Core.Infrastructure;
using NetPro.Core.Infrastructure.Mapper;
using NetPro.TypeFinder;
using NetPro.Web.Core.Helpers;
using NetPro.Web.Core.Infrastructure.Extensions;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace NetPro.Web.Core
{
    public class NetProCoreStartupMin : INetProStartup
    {
        public int Order => int.MinValue;

        public void Configure(IApplicationBuilder application)
        {
        }

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration = null, ITypeFinder typeFinder = null)
        {
            //add controller service
            var mvcBuilder = services.AddControllers();
            string AssemblySkipLoadingPattern = "^Enums.NET|^App.Metrics.Formatters.Ascii|^App.Metrics.Extensions.Hosting|^App.Metrics.Extensions.DependencyInjection|^App.Metrics.Extensions.Configuration|^App.Metrics|^App.Metrics.Core|^App.Metrics.Concurrency|^App.Metrics.Abstractions|^ConsoleTables|^NetPro.TypeFinder|^Com.Ctrip.Framework.Apollo|^Com.Ctrip.Framework.Apollo.Configuration|^NetPro.Core|^Figgle|^NetPro.Startup|^Serilog.Extensions.Logging|^Serilog|^netstandard|^Serilog.Extensions.Hosting|^OpenTracing.Contrib.NetCor|^App.Metrics.AspNetCore|^SkyAPM|^Swashbuckle|^System|^mscorlib|^Microsoft|^AjaxControlToolkit|^Antlr3|^Autofac|^AutoMapper|^Castle|^ComponentArt|^CppCodeProvider|^DotNetOpenAuth|^EntityFramework|^EPPlus|^FluentValidation|^ImageResizer|^itextsharp|^log4net|^MaxMind|^MbUnit|^MiniProfiler|^Mono.Math|^MvcContrib|^Newtonsoft|^NHibernate|^nunit|^Org.Mentalis|^PerlRegex|^QuickGraph|^Recaptcha|^Remotion|^RestSharp|^Rhino|^Telerik|^Iesi|^TestDriven|^TestFu|^UserAgentStringLibrary|^VJSharpCodeProvider|^WebActivator|^WebDev|^WebGrease";

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (!Regex.IsMatch(assembly.FullName, AssemblySkipLoadingPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled) && Regex.IsMatch(assembly.FullName, ".*", RegexOptions.IgnoreCase | RegexOptions.Compiled))
                {
                    //Inject all the assemblies the controller needs,otherwise  "mvcBuilder.PartManager.ApplicationParts" wont't get it
                    mvcBuilder.AddApplicationPart(assembly);
                }
            }
            mvcBuilder.AddControllersAsServices();

            //most of API providers require TLS 1.2 nowadays
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            services.AddScoped<IWebHelper, WebHelper>();
            services.AddHttpContextAccessor();
            //services.AddScoped<IActionContextAccessor, ActionContextAccessor>();
            services.AddHttpClient();

            //日志初始化配置
            services.ConfigureSerilogConfig(configuration);
            //create, initialize and configure the engine
            //var engine = EngineContext.Create().ConfigureServices(services, configuration);
        }
    }

    internal static class _Helper
    {
        /// <summary>
        /// 配置日志组件初始化
        /// </summary>
        /// <param name="configuration"></param>
        internal static void ConfigureSerilogConfig(this IServiceCollection services, IConfiguration configuration)
        {
            Serilog.Log.Logger = new LoggerConfiguration()
              .ReadFrom.Configuration(configuration)
              .CreateLogger();
        }
    }
}
