using NetPro.CsRedis;

namespace XXX.Plugin.Redis
{
    public class RedisStartup : INetProStartup
    {
        public double Order { get; set; } = int.MaxValue;

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration = null, ITypeFinder typeFinder = null)
        {
            //基于NetPro.Web.Api的程序，CsRedis支持自动根据配置文件初始化，如需覆盖默认初始化逻辑可在此重新初始化。
            services.AddCsRedis<SystemTextJsonSerializer>(configuration, GetConnectionString);
        }

        public void Configure(IApplicationBuilder application, IWebHostEnvironment env)
        {
        }

        public List<ConnectionString> GetConnectionString(IServiceProvider serviceProvider)
        {
            return new List<ConnectionString>
            {
                new ConnectionString
                {
                    Key = "2",
                    Value = "192.168.100.187:6379,password=,defaultDatabase=0,poolsize=10,preheat=20,ssl=false,writeBuffer=10240,prefix=key前辍,testcluster=false,idleTimeout=10"
                }
            };
        }
    }
}
