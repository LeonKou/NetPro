using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MQMiddleware;
using MQMiddleware.Configuration;
using NetPro;
using NetPro.Core.Infrastructure;
using NetPro.TypeFinder;
using RabbitMQ.Client;
using System.Collections.Generic;
using System.Linq;

namespace Leon.XXXV2.Api
{
    public class ApiStartup : INetProStartup
    {
        public double Order { get; set; } = 1000;
        public static IFreeSql Fsql { get; private set; }

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration = null, ITypeFinder typeFinder = null)
        {
            services.Scan(scan => scan
              .FromAssemblies(typeFinder.GetAssemblies().Where(s =>
                    s.GetName().Name.EndsWith("Leon.XXXV2.Api")).ToArray())
              .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Service")))
              .AsImplementedInterfaces()
              .WithScopedLifetime());

        }

        public void Configure(IApplicationBuilder application)
        {
        }
    }
}
