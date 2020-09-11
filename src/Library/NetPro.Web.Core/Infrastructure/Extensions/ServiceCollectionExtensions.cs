using NetPro.Core.Configuration;
using NetPro.Core.Infrastructure;
using NetPro.Utility.Helpers;
using NetPro.Web.Core.Filters;
using NetPro.Web.Core.Providers;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Threading;
using System.Net;
using NetPro.Core;
using Microsoft.Extensions.Hosting;
using NetPro.Checker;
using System.Diagnostics;
using System.Text.Json;
using Console = Colorful.Console;
using System.Drawing;
using Serilog;
using System.Text;
using NetPro.Web.Core.Models;
using NetPro.TypeFinder;
using NetPro.ShareRequestBody;
using NetPro.ResponseCache;
using NetPro.Sign;
using FluentValidation;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ApplicationParts;

namespace NetPro.Web.Core.Infrastructure.Extensions
{
    /// <summary>
    /// Represents extensions of IServiceCollection
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add services to the application and configure service provider
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="configuration">Configuration root of the application</param>
        /// <returns>Configured service provider</returns>
        public static (IEngine, NetProOption) ConfigureApplicationServices(this IServiceCollection services,
            IConfiguration configuration, IHostEnvironment hostEnvironment)
        {
            //文件查找组件注入
            services.AddFileProcessService();
            var netProOption = services.ConfigureStartupConfig<NetProOption>(configuration.GetSection(nameof(NetProOption)));
            services.ConfigureStartupConfig<HostingConfig>(configuration.GetSection("Hosting"));
            if (string.IsNullOrWhiteSpace(netProOption.ApplicationName)) netProOption.ApplicationName = hostEnvironment.ApplicationName;

            if (hostEnvironment.EnvironmentName == Environments.Development)
            {
                Console.WriteAscii("Hello NetPro", Color.FromArgb(244, 212, 255));
                //使用dotnet watch run  启动后可以调试此进程id
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss} {hostEnvironment.EnvironmentName}] dotnet process id:{Process.GetCurrentProcess().Id}");
            }

            //most of API providers require TLS 1.2 nowadays
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            services.AddHttpContextAccessor();
            services.AddHttpClient();
            ////create default file provider
            //CoreHelper.DefaultFileProvider = new NetProFileProvider(hostEnvironment);

            //日志初始化配置
            services.ConfigureSerilogConfig(configuration);
            //create, initialize and configure the engine
            var engine = EngineContext.Create();
            engine.ConfigureServices(services, configuration, netProOption);

            if (netProOption.ThreadMinCount > 2)
            {
                if (ThreadPool.SetMinThreads(Environment.ProcessorCount * netProOption.ThreadMinCount, Environment.ProcessorCount * netProOption.ThreadMinCount))
                {
                    ThreadPool.GetMinThreads(out int work, out int comple);
                    ThreadPool.GetAvailableThreads(out int worktemp, out int completemp);
                    Console.WriteLine($"核心数为：{Environment.ProcessorCount}--默认线程最小为：{work}--Available:{worktemp}");
                }
                else
                    Console.WriteLine("最小线程数设置大于系统提供，设置失效！！");
            }

            if (!configuration.GetValue<bool>("Apollo:Enabled", false))
            {
                HealthCheckRegistry.RegisterHealthCheck("apollo", () =>
              {
                  var uri = new Uri(configuration.GetValue<string>("Apollo:MetaServer"));
                  using (var tcpClient = new System.Net.Sockets.TcpClient(uri.Host, uri.Port))
                  {
                      if (tcpClient.Connected)
                      {
                          Console.WriteLine($"pollo:Env：{configuration.GetValue<string>("Apollo:Env")}");
                          Console.WriteLine($"Apollo:Cluster：{configuration.GetValue<string>("Apollo:Cluster")}");
                          return HealthResponse.Healthy($"{uri.Host}:{uri.Port}连接正常--pollo:Env：{configuration.GetValue<string>("Apollo:Env")}--Apollo:Cluster：{configuration.GetValue<string>("Apollo:Cluster")}");
                      }
                      return HealthResponse.Unhealthy($"Apollo{uri.Host}:{uri.Port}连接不正常");
                  }
              });
            }
            return (engine, netProOption);
        }

        /// <summary>
        /// Create, bind and register as service the specified configuration parameters 
        /// </summary>
        /// <typeparam name="TConfig">Configuration parameters</typeparam>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="configuration">Set of key/value application configuration properties</param>
        /// <returns>Instance of configuration parameters</returns>
        public static TConfig ConfigureStartupConfig<TConfig>(this IServiceCollection services, IConfigurationSection configuration) where TConfig : class, new()
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            //bind it to the appropriate section of configuration
            TConfig config = new TConfig();
            configuration.Bind(config);

            //and register it as a service
            services.AddSingleton(config);

            return config;
        }

        /// <summary>
        /// 配置日志组件初始化
        /// </summary>
        /// <param name="configuration"></param>
        public static void ConfigureSerilogConfig(this IServiceCollection services, IConfiguration configuration)
        {
            Serilog.Log.Logger = new LoggerConfiguration()
              .ReadFrom.Configuration(configuration)
              .CreateLogger();
        }

        /// <summary>
        /// Register HttpContextAccessor
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        public static void AddHttpContextAccessor(this IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }

        /// <summary>
        /// Add and configure MVC for the application
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <returns>A builder for configuring MVC services</returns>
        public static IMvcBuilder AddNetProCore(this IServiceCollection services, NetProOption netProOption)
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
                    .Where(part => part.Name.StartsWith($"{netProOption.ProjectPrefix}", StringComparison.InvariantCultureIgnoreCase))
                    .Select(part => part.Assembly);
                configuration.RegisterValidatorsFromAssemblies(assemblies);

                //implicit/automatic validation of child properties 复合对象是否验证
                configuration.ImplicitlyValidateChildProperties = true;
            });

            var corsOrigins = ConvertHelper.ToList<string>(NetProOption.CorsOrigins).ToArray();
            //支持跨域访问
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", (builder) =>
                {
                    if (!string.IsNullOrEmpty(NetProOption.CorsOrigins) && NetProOption.CorsOrigins == "*")//所有域名都可跨域
                    {
                        builder = builder.SetIsOriginAllowed((host) => true);
                    }
                    else
                    {
                        builder = builder.WithOrigins(corsOrigins);
                    }

                    builder.AllowAnyMethod()
                       .AllowAnyHeader()
                       .AllowCredentials();
                });

            });

            //if (NetProOption.APMEnabled)
            //{
            //    mvcBuilder.AddMetrics();
            //}
            return mvcBuilder;
        }

        ///// <summary>
        ///// 新增EFCore性能监控
        ///// </summary>
        ///// <param name="services"></param>
        //public static void AddMiniProfilerEF(this IServiceCollection services)
        //{
        //    var config = services.GetNetProConfig();
        //    if (!config.MiniProfilerEnabled) return;
        //    // profiling
        //    services.AddMiniProfiler(options =>
        //        options.RouteBasePath = "/profiler").AddEntityFramework();
        //}

        /// <summary>
        /// 获取配置信息
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static NetProOption GetNetProConfig(this IServiceCollection services)
        {
            return services.BuildServiceProvider().GetRequiredService<NetProOption>();
        }
    }
}