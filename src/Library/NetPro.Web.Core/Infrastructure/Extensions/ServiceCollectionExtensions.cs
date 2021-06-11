using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NetPro.Checker;
using NetPro.Core.Configuration;
using NetPro.Core.Infrastructure;
using NetPro.TypeFinder;
using Serilog;
using System;
using System.Diagnostics;
using System.Net;
using System.Threading;

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
                Console.Title = hostEnvironment.ApplicationName;
                Console.ForegroundColor = ConsoleColor.Cyan;
                var frameworkName= System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
                
                Console.WriteLine(Figgle.FiggleFonts.Varsity.Render(frameworkName.Substring(0, frameworkName.IndexOf('.'))));
                Console.ResetColor();
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
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss} 核心数为：{Environment.ProcessorCount}--默认线程最小为：{work}--Available:{worktemp}");
                }
                else
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss} 最小线程数设置大于系统提供，设置失效！！");
            }

            if (configuration.GetValue<bool>("Apollo:Enabled", false))
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] apollo已开启");
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Apollo MetaServer={configuration.GetValue<string>("Apollo:MetaServer")}");

                HealthCheckRegistry.RegisterHealthCheck("apollo", () =>
              {
                  var uri = new Uri(configuration.GetValue<string>("Apollo:MetaServer"));
                  using (var tcpClient = new System.Net.Sockets.TcpClient(uri.Host, uri.Port))
                  {
                      if (tcpClient.Connected)
                      {
                          Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] pollo:Env={configuration.GetValue<string>("Apollo:Env")}");
                          Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Apollo:Cluster={configuration.GetValue<string>("Apollo:Cluster")}");
                          return HealthResponse.Healthy($"{uri.Host}:{uri.Port}connection successful; pollo:Env={configuration.GetValue<string>("Apollo:Env")}--Apollo:Cluster={configuration.GetValue<string>("Apollo:Cluster")}");
                      }
                      return HealthResponse.Unhealthy($"Apollo{uri.Host}:{uri.Port} connection failed");
                  }
              });
            }
            else
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] apollo已关闭");
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