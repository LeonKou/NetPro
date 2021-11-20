﻿using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetPro.Core.Configuration;
using NetPro.Core.Infrastructure;
using NetPro.Swagger;
using NetPro.TypeFinder;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace NetPro.Web.Api
{
    /// <summary>
    /// 路由中间件加载app.UseRouting(); 
    /// </summary>
    class RoutingStartup200 : INetProStartup
    {
        /// <summary>
        /// Add and configure any of the middleware
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="configuration">Configuration root of the application</param>
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration, ITypeFinder typeFinder)
        {
            //支持IIS
            services.Configure<IISOptions>(options =>
            {
                options.ForwardClientCertificate = false;
            });

            //配置 mvc服务
            var serviceProvider = services.BuildServiceProvider();
            var netproOption = serviceProvider.GetService<NetProOption>();

            var mvcBuilder = services.AddControllers(config =>
            {
                //config.Filters.Add(typeof(CustomAuthorizeFilter));//用户权限验证过滤器                                                                    
                                                                  //同类型的过滤按添加先后顺序执行,第一个最先执行；ApiController特性下，
                                                                  //过滤器无法挡住过滤验证失败，故无法统一处理，只能通过ConfigureApiBehaviorOptions 
                                                                  
                //增加路由统一前缀
                if (!string.IsNullOrWhiteSpace(netproOption.RoutePrefix))
                {
                    //参考：https://gist.github.com/itavero/2b40dfb476bebff756da35b5c7ff7384
                    //https://www.strathweb.com/2016/06/global-route-prefix-with-asp-net-core-mvc-revisited/
                    //https://benjii.me/2016/08/global-routes-for-asp-net-core-mvc/
                    config.Conventions.Insert(0, new GlobalRoutePrefixConvention(netproOption.RoutePrefix));
                }

                config.Filters.Add(typeof(BenchmarkActionFilter));//接口性能监控过滤器
            }).ConfigureApiBehaviorOptions(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    foreach (var keyModelStatePair in context.ModelState)
                    {
                        var key = keyModelStatePair.Key;
                        var errors = keyModelStatePair.Value.Errors;
                        if (errors != null && errors.Count > 0)
                        {
                            stringBuilder.Append(errors[0].ErrorMessage);
                        }
                    }
                    return new BadRequestObjectResult(new ResponseResult { Code = -1, Msg = $"数据验证失败--详情：{stringBuilder}" })
                    {
                        ContentTypes = { "application/problem+json", "application/problem+xml" }
                    };
                };
            });

            //mvc binder对象转换支持空字符串.如果传入空字符串为转成空字符串，默认会转成null
            mvcBuilder.AddMvcOptions(options => options.ModelMetadataDetailsProviders.Add(new CustomMetadataProvider()));

            //MVC now serializes JSON with camel case names by default, use this code to avoid it
            //mvcBuilder.AddJsonOptions(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver());


            //register all available validators from netpro assemblies   
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            string AssemblySkipLoadingPattern = "^Enums.NET|^App.Metrics.Formatters.Ascii|^App.Metrics.Extensions.Hosting|^App.Metrics.Extensions.DependencyInjection|^App.Metrics.Extensions.Configuration|^App.Metrics|^App.Metrics.Core|^App.Metrics.Concurrency|^App.Metrics.Abstractions|^ConsoleTables|^NetPro.TypeFinder|^Com.Ctrip.Framework.Apollo|^Com.Ctrip.Framework.Apollo.Configuration|^NetPro.Core|^Figgle|^NetPro.Startup|^Serilog.Extensions.Logging|^Serilog|^netstandard|^Serilog.Extensions.Hosting|^OpenTracing.Contrib.NetCor|^App.Metrics.AspNetCore|^SkyAPM|^Swashbuckle|^System|^mscorlib|^Microsoft|^AjaxControlToolkit|^Antlr3|^Autofac|^AutoMapper|^Castle|^ComponentArt|^CppCodeProvider|^DotNetOpenAuth|^EntityFramework|^EPPlus|^FluentValidation|^ImageResizer|^itextsharp|^log4net|^MaxMind|^MbUnit|^MiniProfiler|^Mono.Math|^MvcContrib|^Newtonsoft|^NHibernate|^nunit|^Org.Mentalis|^PerlRegex|^QuickGraph|^Recaptcha|^Remotion|^RestSharp|^Rhino|^Telerik|^Iesi|^TestDriven|^TestFu|^UserAgentStringLibrary|^VJSharpCodeProvider|^WebActivator|^WebDev|^WebGrease";
            var assembliesResult = new List<Assembly>();
            foreach (var item in assemblies)
            {
                if (!Regex.IsMatch(item.FullName, AssemblySkipLoadingPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled) && Regex.IsMatch(item.FullName, ".*", RegexOptions.IgnoreCase | RegexOptions.Compiled))
                {
                    assembliesResult.Add(item);
                }
            }

            //add fluent validation
            mvcBuilder.AddFluentValidation(configuration =>
            {            
                configuration.RegisterValidatorsFromAssemblies(assembliesResult);

                //implicit/automatic validation of child properties 复合对象是否验证
                configuration.ImplicitlyValidateChildProperties = true;
            });

            //Inject all the assemblies the controller needs,otherwise  "mvcBuilder.PartManager.ApplicationParts" wont't get it
            assembliesResult.ForEach(a => mvcBuilder.AddApplicationPart(a));

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


            //路由小写
            services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
            services.AddNetProSwagger(configuration);
            //services.SwaggerConfigureOauth2(configuration);
        }

        public void Configure(IApplicationBuilder application)
        {
            application.Use(async (context, next) =>
            {
                context.Request.EnableBuffering();
                await next();
            });

            application.UseNetProSwagger();
            //TODO流量分析等其他中间件
            //
            application.UseRouting();
        }

        /// <summary>
        /// Gets order of this startup configuration implementation
        /// </summary>
        public double Order { get; set; } = 200; //Routing should be loaded last
    }

    /// <summary>
    /// 全局路由前缀
    /// </summary>
    public class GlobalRoutePrefixConvention : IApplicationModelConvention
    {
        private readonly string _routePrefix;
        public GlobalRoutePrefixConvention(string routePrefix)
        {
            _routePrefix = routePrefix;
        }
        public void Apply(ApplicationModel application)
        {
            var centralPrefix = new AttributeRouteModel(new RouteAttribute(_routePrefix));
            foreach (var controller in application.Controllers)
            {
                var routeSelector = controller.Selectors.FirstOrDefault(x => x.AttributeRouteModel != null);

                if (routeSelector != null)
                {
                    routeSelector.AttributeRouteModel = AttributeRouteModel.CombineAttributeRouteModel(centralPrefix,
                        routeSelector.AttributeRouteModel);
                }
                else
                {
                    controller.Selectors.Add(new SelectorModel() { AttributeRouteModel = centralPrefix });
                }
            }
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
