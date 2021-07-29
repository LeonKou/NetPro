using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetPro.Core.Infrastructure;
using NetPro.TypeFinder;
using NetPro.Proxy;

namespace NetPro.Web.Core
{
    public class ApiProxyStartup2000 : INetProStartup
    {
        public int Order => 2000;

        public void Configure(IApplicationBuilder application)
        {
        }

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration = null, ITypeFinder typeFinder = null)
        {
            services.AddHttpProxy(configuration, typeFinder, configuration.GetValue<string>("MicroServicesEndpoint:Assembly", string.Empty));
        }
    }
}
