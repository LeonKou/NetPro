using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetPro.Core.Infrastructure;
using NetPro.TypeFinder;

namespace Leon.XXX.Proxy
{
    public class XXXApiProxyStartup : INetProStartup
    {
        public int Order => 900;

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration = null, ITypeFinder typeFinder = null)
        {
            //services.AddSingleton<IHttpApiFactory<IExampleProxy>, HttpApiFactory<IExampleProxy>>(p =>
            //{
            //	return new HttpApiFactory<IExampleProxy>().ConfigureHttpApiConfig(c =>
            //	{
            //		c.HttpHost = new Uri("http://localhost:5000/");
            //		c.LoggerFactory = p.GetRequiredService<ILoggerFactory>();
            //	});
            //});

            //services.AddTransient<IExampleProxy>(p =>
            //{
            //	var factory = p.GetRequiredService<IHttpApiFactory<IExampleProxy>>();
            //	return factory.CreateHttpApi();
            //});
        }

        public void Configure(IApplicationBuilder application)
        {

        }
    }
}
