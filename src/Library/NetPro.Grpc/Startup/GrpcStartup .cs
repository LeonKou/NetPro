using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.NetPro;
using System.Reflection;

namespace NetPro.Grpc
{
    public class GrpcStartup : INetProStartup, System.NetPro.Startup.__._
    {
        public double Order { get; set; } = 0;

        public void Configure(IApplicationBuilder application, IWebHostEnvironment env)
        {
            application.UseRouting();

            application.UseEndpoints(endpoints =>
            {
                GrpcServiceExtension.AddGrpcServices(endpoints, new string[] { $"{Assembly.GetEntryAssembly().GetName().Name}" });
            });
        }

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration = null, ITypeFinder typeFinder = null)
        {
            services.AddGrpc();

            //日志初始化配置
            services.ConfigureSerilogConfig(configuration);
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
