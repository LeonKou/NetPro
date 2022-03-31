using NetPro.MongoDb;

namespace XXX.Plugin.MongoDB
{
    public class MongoDBDemoStartup : INetProStartup
    {
        public double Order { get; set; } = int.MaxValue;

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration = null, ITypeFinder typeFinder = null)
        {
            services.AddMongoDb<CustomConnector>().Build(configuration);
        }
        
        public void Configure(IApplicationBuilder application, IWebHostEnvironment env)
        {
        }
        public class CustomConnector : IConnectionsFactory
        {
            public IList<ConnectionString> GetConnectionStrings()
            {
                var connector = new List<ConnectionString>();
                connector.Add(new ConnectionString { Key = "2", Value = "mongodb://192.168.100.187:27017/netprodemo2" });
                return connector;
            }
        }
    }
}