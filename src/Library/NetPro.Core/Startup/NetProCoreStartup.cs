using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NetPro.Core.Configuration;
using System;
using System.NetPro;
using System.Reflection;
using System.Threading;

namespace NetPro.Core.Startup
{
    internal sealed class NetProCoreStartup : INetProStartup, System.NetPro.Startup.__._
    {
        //public double Order = 0;

        public double Order { get; set; } = 0;

        public void Configure(IApplicationBuilder application, IWebHostEnvironment env)
        {
            //转发头数据
            application.UseForwardedHeaders(new ForwardedHeadersOptions { ForwardedHeaders = ForwardedHeaders.All });
        }

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration = null, ITypeFinder typeFinder = null)
        {
            services.InterfaceDependency();
            var netProOption = services.ConfigureStartupConfig<NetProOption>(configuration.GetSection(nameof(NetProOption)));
            services.ConfigureStartupConfig<HostingConfig>(configuration.GetSection("Hosting"));

            if (string.IsNullOrWhiteSpace(netProOption.ApplicationName))
            {
                var serviceProvider = services.BuildServiceProvider();
                //reset ApplicationName
                serviceProvider.GetRequiredService<IHostEnvironment>().ApplicationName = Assembly.GetEntryAssembly().GetName().Name;
                var hostEnvironment = serviceProvider.GetRequiredService<IHostEnvironment>();

                netProOption.ApplicationName = hostEnvironment.ApplicationName;
            }

            //--------------- AutoMapper

            //AddAutoMapper(services, typeFinder);

            //----------------
            //由runtimeconfig.template.json 提供配置
            /*
              {
                "configProperties": {
                 "System.GC.Server": true,//站点专用配置,默认false
                 "System.GC.HeapHardLimit": 83886080,堆上限，单位字节
                 "System.GC.HeapHardLimitPercent": 2,//堆上限，单位百分比
                 "System.GC.LOHThreshold": 1048576,//大对象定义，单位字节
                 "System.GC.Concurrent": true,//后台回收，默认true
                 "System.Runtime.Serialization.EnableUnsafeBinaryFormatterSerialization": false,
                 "System.Threading.ThreadPool.MinThreads": 200
               }
            }
             */
            ThreadPool.GetMinThreads(out int work, out int comple);
            ThreadPool.GetAvailableThreads(out int worktemp, out int completemp);
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss} 核心数为：{Environment.ProcessorCount}--默认线程最小为：{work}--Available:{worktemp}");

            if (configuration.GetValue<bool>("Apollo:Enabled", false))
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] apollo已开启");
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Apollo MetaServer={configuration.GetValue<string>("Apollo:MetaServer")}");

                var uri = new Uri(configuration.GetValue<string>("Apollo:MetaServer"));
                using (var tcpClient = new System.Net.Sockets.TcpClient(uri.Host, uri.Port))
                {
                    if (tcpClient.Connected)
                    {
                        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] pollo:Env={configuration.GetValue<string>("Apollo:Env")}");
                        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Apollo:Cluster={configuration.GetValue<string>("Apollo:Cluster")}");
                        Console.WriteLine($"{uri.Host}:{uri.Port}connection successful; pollo:Env={configuration.GetValue<string>("Apollo:Env")}--Apollo:Cluster={configuration.GetValue<string>("Apollo:Cluster")}");
                    }
                    Console.WriteLine($"Apollo{uri.Host}:{uri.Port} connection failed");
                }
            }
            else
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] apollo已关闭");
            }
        }
    }

    internal static class _Helper
    {
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
    }
}
