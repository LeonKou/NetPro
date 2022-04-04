using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetPro.EasyNetQ;

namespace XXX.Plugin.EasyNetQ
{
    public class XXXEasyNetQ : INetProStartup
    {
        public double Order { get; set; } = int.MaxValue;

        public void ConfigureServices(IServiceCollection services, IConfiguration? configuration = null, ITypeFinder? typeFinder = null)
        {
            services.AddEasyNetQ(GetConnectionString);
        }

        public void Configure(IApplicationBuilder application, IWebHostEnvironment env)
        {
        }

        public IList<ConnectionString> GetConnectionString(IServiceProvider serviceProvider)
        {
            var connector = new List<ConnectionString>
            {
                new ConnectionString { Key = "2", Value = "host=192.168.100.187:5672;virtualHost=my_vhost;username=admin;password=admin;timeout=60" }
            };
            return connector;
        }
    }
}
