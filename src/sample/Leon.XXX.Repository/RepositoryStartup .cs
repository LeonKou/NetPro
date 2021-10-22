using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetPro;
using NetPro.TypeFinder;
using System.Linq;

namespace Leon.XXXV2.Api
{
    public class RepositoryStartup : INetProStartup
    {
        public double Order { get; set; } = 1000;
        public static IFreeSql Fsql { get; private set; }

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration = null, ITypeFinder typeFinder = null)
        {
            services.Scan(scan => scan
              .FromAssemblies(this.GetType().Assembly)
              .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Repository")))
              .AsImplementedInterfaces()
              .WithScopedLifetime());
        }

        public void Configure(IApplicationBuilder application)
        {
        }
    }
}
