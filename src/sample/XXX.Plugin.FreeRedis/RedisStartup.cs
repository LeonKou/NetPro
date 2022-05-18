using NetPro.FreeRedis;

namespace XXX.Plugin.FreeRedis
{
    public class RedisStartup : INetProStartup
    {
        public double Order { get; set; } = int.MaxValue;

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration = null, ITypeFinder typeFinder = null)
        {
            // 基于NetPro.Web.Api的程序，FreeRedis支持自动根据配置文件初始化，如需覆盖默认初始化逻辑可在此重新初始化。
            services.AddFreeRedis(configuration, GetConnectionString);
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
                    Value = "127.0.0.1:6379,password=123,defaultDatabase=0,ssl=false,prefix=key前辍"
                }
            };
        }
    }
}
