using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetPro.Core.Configuration;
using NetPro.Core.Infrastructure;
using NetPro.TypeFinder;
using NetPro.Utility.Helpers;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using NetPro.Proxy;

namespace NetPro.Web.Api
{
    public class ApiProxyStartup : INetProStartup
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
