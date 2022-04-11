using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;
using System.NetPro;
using System.Reflection;

namespace NetPro.Grpc
{
    internal sealed class GrpcServerStartup : INetProStartup, System.NetPro.Startup.__._
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
        }
    }
   
}
