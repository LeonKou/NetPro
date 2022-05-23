using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.NetPro;
using System.Reflection;
using System.Threading;

namespace NetPro.SerilogStartup
{
    internal sealed class NetProSerilogStartup //: INetProStartup, System.NetPro.Startup.__._
    {
        public double Order { get; set; } = 0;

        public void Configure(IApplicationBuilder application, IWebHostEnvironment env)
        {
            //application.UseSerilogRequestLogging();
        }

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration = null, ITypeFinder typeFinder = null)
        {
        }
    }
}
