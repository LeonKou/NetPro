using FluentValidation.AspNetCore;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NetPro.Checker;
using NetPro.Core.Configuration;
using NetPro.Core.Infrastructure;
using NetPro.CsRedis;
using NetPro.ResponseCache;
using NetPro.TypeFinder;
using NetPro.Web.Api;
using NetPro.Web.Core.Infrastructure.Extensions;
using NetPro.Web.Core.Models;
using NetPro.Web.Core.Providers;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace NetPro.Web.Core.Infrastructure
{
    /// <summary>
    /// 配置应用程序启动时MVC需要的中间件
    /// </summary>
    public class NetProCoreStartup : INetProStartup
    {
        /// <summary>
        /// Add and configure any of the middleware
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="configuration">Configuration root of the application</param>
        /// <param name="typeFinder"></param>
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration, ITypeFinder typeFinder)
        {
            var serviceProvider = services.BuildServiceProvider();
            var redisCacheOption = serviceProvider.GetService<RedisCacheOption>();
            var netproOption = serviceProvider.GetService<NetProOption>();

            //配置 ef性能监控
            //services.AddMiniProfilerEF();
            //配置 mvc服务

            AddNetProCore(services, netproOption);
            //services.AddNetProCore(netproOption);

            //健康检查
            if (!netproOption?.EnabledHealthCheck ?? false)
                return;

            var healthbuild = services.AddHealthChecks();
            //if (!string.IsNullOrWhiteSpace(mongoDbOptions?.ConnectionString))
            //    healthbuild.AddMongoDb(mongoDbOptions.ConnectionString, tags: new string[] { "mongodb" });
            //foreach (var item in redisCacheOption?.Endpoints ?? new List<ServerEndPoint>())
            //{
            //    healthbuild.AddRedis($"{item.Host}:{item.Port},password={redisCacheOption.Password}", name: $"redis-{Guid.NewGuid()}");
            //}

            //services.AddHealthChecksUI();
        }

        /// <summary>
        /// Configure the using of added middleware
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public void Configure(IApplicationBuilder application)
        {
            var config = EngineContext.Current.Resolve<NetProOption>();
            //if (config.MiniProfilerEnabled)
            //{
            //    //add MiniProfiler
            //    application.UseMiniProfiler();
            //    application.UseMiddleware<MiniProfilerMiddleware>();
            //}

            if (!config.EnabledHealthCheck) return;

            application.UseHealthChecks("/health", new HealthCheckOptions()
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            //application.UseHealthChecksUI(s => s.UIPath = "/ui");

            application.UseCheck();
        }

        /// <summary>
        /// Add and configure MVC for the application
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <returns>A builder for configuring MVC services</returns>
        public IMvcBuilder AddNetProCore(IServiceCollection services, NetProOption netProOption)
        {
            //TODO 流量分析

            //响应缓存
            services.AddResponseCachingExtension();

            var NetProOption = services.GetNetProConfig();

            //支持IIS
            services.Configure<IISOptions>(options =>
            {
                options.ForwardClientCertificate = false;
            });
            //替换控制器所有者
            services.Replace(ServiceDescriptor.Transient<IControllerActivator, ServiceBasedControllerActivator>());
            //add basic MVC feature
            //AddMvc
            var mvcBuilder = services.AddControllers(config =>
            {
                //增加路由统一前缀
                if (!string.IsNullOrWhiteSpace(netProOption.RoutePrefix))
                {
                    //参考：https://gist.github.com/itavero/2b40dfb476bebff756da35b5c7ff7384
                    //https://www.strathweb.com/2016/06/global-route-prefix-with-asp-net-core-mvc-revisited/
                    //https://benjii.me/2016/08/global-routes-for-asp-net-core-mvc/
                    config.Conventions.Insert(0, new GlobalRoutePrefixConvention(netProOption.RoutePrefix));
                }

                //config.Filters.Add(typeof(ShareResponseBodyFilter));//请求响应body
                //config.Filters.Add(typeof(NetProExceptionFilter));//自定义全局异常过滤器
                config.Filters.Add(typeof(BenchmarkActionFilter));//接口性能监控过滤器

                //if (NetProOption.PermissionEnabled)
                //{
                //    config.Filters.Add(typeof(PermissionActionFilter));//用户权限验证过滤器
                //}

                //config.Filters.Add(typeof(ReqeustBodyFilter));//请求数据过滤器
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

            //add fluent validation
            mvcBuilder.AddFluentValidation(configuration =>
            {
                //register all available validators from netpro assemblies               

                var assemblies = mvcBuilder.PartManager.ApplicationParts
                    .OfType<AssemblyPart>()
                    //.Where(part => part.Name.StartsWith($"{netProOption.ProjectPrefix}", StringComparison.InvariantCultureIgnoreCase))
                    .Select(part => part.Assembly);
                string AssemblySkipLoadingPattern = "^OpenTracing.Contrib.NetCor|^App.Metrics.AspNetCore|^SkyAPM|^Swashbuckle|^System|^mscorlib|^Microsoft|^AjaxControlToolkit|^Antlr3|^Autofac|^AutoMapper|^Castle|^ComponentArt|^CppCodeProvider|^DotNetOpenAuth|^EntityFramework|^EPPlus|^FluentValidation|^ImageResizer|^itextsharp|^log4net|^MaxMind|^MbUnit|^MiniProfiler|^Mono.Math|^MvcContrib|^Newtonsoft|^NHibernate|^nunit|^Org.Mentalis|^PerlRegex|^QuickGraph|^Recaptcha|^Remotion|^RestSharp|^Rhino|^Telerik|^Iesi|^TestDriven|^TestFu|^UserAgentStringLibrary|^VJSharpCodeProvider|^WebActivator|^WebDev|^WebGrease";
                var assembliesResult = new List<Assembly>();
                foreach (var item in assemblies)
                {
                    if (!Regex.IsMatch(item.FullName, AssemblySkipLoadingPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled) && Regex.IsMatch(item.FullName, ".*", RegexOptions.IgnoreCase | RegexOptions.Compiled))
                    {
                        assembliesResult.Add(item);
                    }
                }
                configuration.RegisterValidatorsFromAssemblies(assembliesResult);
                //implicit/automatic validation of child properties 复合对象是否验证
                configuration.ImplicitlyValidateChildProperties = true;
            });

            //if (NetProOption.APMEnabled)
            //{
            //    mvcBuilder.AddMetrics();
            //}
            return mvcBuilder;
        }

        /// <summary>
        /// Gets order of this startup configuration implementation
        /// </summary>
        public int Order => 1000;

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
    }
}
