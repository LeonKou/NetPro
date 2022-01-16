using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using NetMQ;
using NetMQ.Sockets;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

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
                    ////发布订阅
                    //Task.Run(() =>
                    //{
                    //    using (var publisher = new PublisherSocket())
                    //    {
                    //        //发布是由于本机承载故配回环地址即可
                    //        publisher.Bind("tcp://*:81");

                    //        while (true)
                    //        {
                    //            publisher
                    //           .SendMoreFrame("A") // Topic
                    //           .SendFrame(DateTimeOffset.Now.ToString()); // Message
                    //            Thread.Sleep(1000);
                    //        }
                    //    }
                    //});

                    //推拉
                    Task.Run(() =>
                    {
                        using (var pushSocket = new PushSocket())
                        {
                            //推要指定远程地址，故不能使用回环地址
                            pushSocket.Connect("tcp://localhost:82");
                            while (true)
                            {
                                pushSocket.SendFrame("Hello Clients");
                                Console.WriteLine("Hello Clients");
                                Thread.Sleep(1000);
                            }
                        }
                    });

                    //var option = new RedisCacheOption();
                    //var tt = hostContext.Configuration.GetSection("Redis").Get<RedisCacheOption>();
                    //hostContext.Configuration.Bind("Redis", option);
                    Console.WriteLine("true");
                })
                .Build();

            host.Run();
        }
    }
}
