using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using RedisManager;
using System.IO;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = new HostBuilder()
                .ConfigureHostConfiguration(configHost =>
                {
                    configHost.SetBasePath(Directory.GetCurrentDirectory());
                    configHost.AddEnvironmentVariables("ASPNETCORE_");
                })
                .ConfigureAppConfiguration((hostContext, configApp) =>
                {
                    configApp.AddJsonFile("appsettings.json", true);
                    configApp.AddJsonFile($"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json", true);
                    configApp.AddEnvironmentVariables();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    var option = new RedisCacheOption();
                    var tt = hostContext.Configuration.GetSection("Redis").Get<RedisCacheOption>();
                    hostContext.Configuration.Bind("Redis", option);

                })
                .Build();

            host.Run();
        }
    }
}
