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
            // 使用自定义的连接字符串获取逻辑覆盖默认的连接字符串加载逻辑
            services.AddTdengineDb(GetConnectionString);
        }

        public void Configure(IApplicationBuilder application, IWebHostEnvironment env)
        {
        }

        public List<ConnectionString> GetConnectionString(IServiceProvider serviceProvider)
        {
            return new List<ConnectionString>
            {
                new ConnectionString()
                {
                    Key ="remotekey",
                    Value = "Data Source=h26.taosdata.com;DataBase=db_netpro;Username=root;Password=taosdata;Port=6030"
                }
            };
        }
    }
}