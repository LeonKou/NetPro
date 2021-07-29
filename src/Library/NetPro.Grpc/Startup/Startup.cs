using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetPro.Core.Infrastructure;
using NetPro.TypeFinder;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NetPro.Grpc.Startup
{
    public class Startup : INetProStartup
    {
        public int Order => 0;

        public void Configure(IApplicationBuilder application)
        {
            application.UseRouting();

            application.UseEndpoints(endpoints =>
            {
                GrpcServiceExtension.AddGrpcServices(endpoints,new string[] {$"{Assembly.GetExecutingAssembly().GetName().Name}" });
                //endpoints.MapGrpcService<GreeterService>();
            });
        }

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration = null, ITypeFinder typeFinder = null)
        {
            services.AddGrpc();
        }
    }
}
