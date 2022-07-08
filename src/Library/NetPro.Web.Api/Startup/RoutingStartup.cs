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

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.NetPro;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace NetPro.Web.Api
{
    /// <summary>
    /// 路由中间件加载app.UseRouting(); 
    /// </summary>
    internal sealed class RoutingStartup : INetProStartup, System.NetPro.Startup.__._
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
                //是否允许Body为空，默认为false不允许为空
                config.AllowEmptyInputInBodyModelBinding = true;
                foreach (var formatter in config.InputFormatters)
                {
                    if (formatter.GetType() == typeof(SystemTextJsonInputFormatter))
                    {   //增加默认支持的ContentType
                        var supportedMediaTypes = ((SystemTextJsonInputFormatter)formatter).SupportedMediaTypes;
                        supportedMediaTypes.Add(Microsoft.Net.Http.Headers.MediaTypeHeaderValue.Parse("text/plain"));
                        //supportedMediaTypes.Add(Microsoft.Net.Http.Headers.MediaTypeHeaderValue.Parse("multipart/form-data"));
                    }
                }

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
                    return new BadRequestObjectResult(
                        $"validation errors：{stringBuilder}" //return content-type: text/plain;
                                                             //new ResponseResult { Code = -1, Msg = $"数据验证失败--详情：{stringBuilder}" }
                        )
                    {
                        //ContentTypes = { "text/plain; charset=utf-8", "application/problem+json", "application/problem+xml" },                        
                    };
                };
            });

            //mvc binder对象转换支持空字符串.如果传入空字符串为转成空字符串，默认会转成null
            mvcBuilder.AddMvcOptions(options => options.ModelMetadataDetailsProviders.Add(new CustomMetadataProvider()));

            //MVC now serializes JSON with camel case names by default, use this code to avoid it
            //mvcBuilder.AddJsonOptions(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver());

            var assemblies = typeFinder.GetAssemblies();
            foreach (var assembly in assemblies)//AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly.EntryPoint != null || assembly.GetName().Name.Contains(".Plugin."))//The plug-in identifier
                {
                    mvcBuilder.AddApplicationPart(assembly);
                }
                //if (!Regex.IsMatch(assembly.FullName, AssemblySkipLoadingPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled) && Regex.IsMatch(assembly.FullName, ".*", RegexOptions.IgnoreCase | RegexOptions.Compiled))
                //{
                //    //Inject all the assemblies the controller needs,otherwise  "mvcBuilder.PartManager.ApplicationParts" wont't get it
                //    mvcBuilder.AddApplicationPart(assembly);
                //}
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


            //路由小写
            services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
            //services.AddNetProSwagger(configuration);
            //services.SwaggerConfigureOauth2(configuration);
        }

        public void Configure(IApplicationBuilder application, IWebHostEnvironment env)
        {
            //application.Use(async (context, next) =>
            //{
            //    context.Request.EnableBuffering();
            //    await next();
            //});

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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="routePrefix"></param>
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
        /// 已移除，由程序自行决定使用日志驱动
        /// </summary>
        /// <param name="configuration"></param>
        internal static void ConfigureSerilogConfig(this IServiceCollection services, IConfiguration configuration)
        {
            //Serilog.Log.Logger = new Serilog.LoggerConfiguration()
            //  .ReadFrom.Configuration(configuration)
            //  .CreateLogger();
        }
    }
}
