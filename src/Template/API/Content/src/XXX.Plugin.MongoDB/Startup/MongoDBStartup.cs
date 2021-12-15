using MongoDB.Driver;

namespace XXX.Plugin.MongoDBS
{
    public class MongoDBStartup : INetProStartup
    {
        public double Order { get; set; } = int.MaxValue;

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration = null, ITypeFinder typeFinder = null)
        {
            //var client = new MongoClient("mongodb://localhost:<port>");
            //var database = client.GetDatabase("test");
            //IdleBus<MongoClient> idleBus = new IdleBus<MongoClient>(TimeSpan.FromSeconds(10));
        }

        public void Configure(IApplicationBuilder application, IWebHostEnvironment env)
        {
        }
    }
}
