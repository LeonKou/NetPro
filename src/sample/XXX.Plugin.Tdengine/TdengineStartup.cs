using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetPro.Tdengine;

namespace XXX.Plugin.Tdengine
{
    public class TdengineStartup : INetProStartup
    {
        public double Order { get; set; } = 2;

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration = null, ITypeFinder typeFinder = null)
        {
            //基于NetPro.Web.Api的程序，使用自定义的配置获取逻辑覆盖默认的配置加载逻辑
            services.AddTdengineDb(GetTdengineDbOption);
        }

        public void Configure(IApplicationBuilder application, IWebHostEnvironment env)
        {
        }

        public TdengineOption GetTdengineDbOption(IServiceProvider serviceProvider)
        {
            return new TdengineOption
            {
                ConnectionString = new List<ConnectionString>
                {
                    new ConnectionString()
                    {
                        Key ="remotekey",
                        Value = "Data Source=h26.taosdata.com;DataBase=db_netpro;Username=root;Password=taosdata;Port=6030"
                    }
                }
            };
        }
    }
}
