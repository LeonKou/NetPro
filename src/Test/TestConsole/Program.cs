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
                    //发布订阅
                    Task.Run(() =>
                    {
                        //https://netmq.readthedocs.io/en/latest/
                        using (var publisher = new PublisherSocket())
                        {
                            //发布是由于本机承载故配回环地址即可
                            //发布者优先使用bind方法；订阅者和拉取侧优先使用Connect;发布者和推送者优先使用回环地址
                            publisher.Bind("tcp://*:81");
                            while (true)
                            {
                                publisher
                               .SendMoreFrame("A:b:g") // Topic
                               .SendFrame($"A:b:g--{DateTimeOffset.Now.ToString()}"); // Message
                                Console.WriteLine("发布队列-A:b:g-Hello Clients");
                                //Thread.Sleep(1000);

                               // publisher
                               //.SendMoreFrame("A:c:g") // Topic
                               //.SendFrame($"A:c:g--{DateTimeOffset.Now.ToString()}"); // Message
                               // Console.WriteLine("发布队列-A:c:g-Hello Clients");
                            }
                        }
                    });

                    ////推拉
                    //Task.Run(() =>
                    //{
                    //    using (var pushSocket = new PushSocket())
                    //    {
                    //        //发布是由于本机承载故配回环地址即可
                    //        //发布者优先使用bind方法
                    //        pushSocket.Bind("tcp://*:82");
                    //        while (true)
                    //        {
                    //            pushSocket.SendFrame("Hello Clients");
                    //            Console.WriteLine("推送-Hello Clients");
                    //        }
                    //    }
                    //});

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
