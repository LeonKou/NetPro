using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetPro.Pulsar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.NetPro;
using System.Text;
using System.Threading.Tasks;
using XXX.Plugin.Pulsar.TaskService;

namespace XXX.Plugin.Pulsar
{
    public class PulsarStartup : INetProStartup
    {
        public double Order { get; set; } = int.MaxValue;

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration = null, ITypeFinder typeFinder = null)
        {
            services.AddPulsarClient(configuration);
            services.AddHostedService<PulsarTask>();
        }

        public void Configure(IApplicationBuilder application, IWebHostEnvironment env)
        {
        }
    }
}
