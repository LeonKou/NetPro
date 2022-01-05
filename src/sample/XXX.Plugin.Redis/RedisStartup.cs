namespace XXX.Plugin.Redis
{
    public class RedisStartup : INetProStartup
    {
        public double Order { get; set; } = int.MaxValue;

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration = null, ITypeFinder typeFinder = null)
        {
            //基于NetPro.Web.Api的程序，CsRedis支持自动根据配置文件初始化，如需覆盖默认初始化逻辑可在此重新初始化。
        }

        public void Configure(IApplicationBuilder application, IWebHostEnvironment env)
        {
        }
    }
}
