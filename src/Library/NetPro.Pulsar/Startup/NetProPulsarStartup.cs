using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.NetPro;

namespace NetPro.Pulsar.Startup
{
    internal sealed class NetProPulsarStartup : INetProStartup, System.NetPro.Startup.__._
    {
        public double Order { get; set; } = 1000;

        public void Configure(IApplicationBuilder application, IWebHostEnvironment env)
        {
        }

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration = null, ITypeFinder typeFinder = null)
        {
            //pulsar注入
            services.AddPulsarClient(configuration);
        }
    }
}
