using MongoDB.Driver;
using MongoDbGenericRepository;
using System.Collections.Concurrent;
using System.NetPro;

namespace XXX.Plugin.MongoDB
{
    public class MongoDBDemoStartup : INetProStartup
    {
        public double Order { get; set; } = int.MaxValue;

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration = null, ITypeFinder typeFinder = null)
        {
            services.AddMongoDb(configuration);
        }

        public void Configure(IApplicationBuilder application, IWebHostEnvironment env)
        {
        }
    }
}
