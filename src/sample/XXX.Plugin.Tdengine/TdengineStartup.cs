using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetPro.Tdengine;
using WebApiClientCore.Serialization.JsonConverters;

namespace XXX.Plugin.Tdengine
{
    public class TdengineStartup : INetProStartup
    {
        public double Order { get; set; } = 2;

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration = null, ITypeFinder typeFinder = null)
        {
            // 使用自定义的连接字符串获取逻辑覆盖默认的连接字符串加载逻辑
            services.AddTdengineDb(GetConnectionString);

            var section = configuration.GetSection($"Remoting:{nameof(ITaosProxy)}");

            services.AddHttpApi<ITaosProxy>().ConfigureHttpApi(section).ConfigureHttpApi(o =>
            {
                // 符合国情的不标准时间格式，有些接口就是这么要求必须不标准
                o.JsonSerializeOptions.Converters.Add(new JsonDateTimeConverter("yyyy-MM-dd HH:mm:ss"));
            });
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