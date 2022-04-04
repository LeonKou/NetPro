using NetPro.MongoDb;

namespace XXX.Plugin.MongoDB
{
    public class MongoDBDemoStartup : INetProStartup
    {
        public double Order { get; set; } = int.MaxValue;

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration = null, ITypeFinder typeFinder = null)
        {
            // 使用自定义的连接字符串获取逻辑覆盖默认的连接字符串加载逻辑
            services.AddMongoDb(GetConnectionString);
        }

        public void Configure(IApplicationBuilder application, IWebHostEnvironment env)
        {
        }

        public IList<ConnectionString> GetConnectionString(IServiceProvider serviceProvider)
        {
            var connector = new List<ConnectionString>();
            connector.Add(new ConnectionString { Key = "2", Value = "mongodb://192.168.100.187:27017/netprodemo2" });
            return connector;
        }
    }
}