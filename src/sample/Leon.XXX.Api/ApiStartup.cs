using Leon.XXX.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetPro.Core.Infrastructure;
using NetPro.Dapper;

namespace Leon.XXX.Api
{
    public class ApiStartup : INetProStartup
    {
        public int Order => 900;

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration = null, ITypeFinder typeFinder = null)
        {
            //services.AddTransient(s =>
            //new DefaultDapperContext(configuration.GetValue<string>("DatabaseConnection")
            //, DataProvider.Mysql));//原生自带DIf方式注入
        }

        public void Configure(IApplicationBuilder application)
        {
        }
    }
}
