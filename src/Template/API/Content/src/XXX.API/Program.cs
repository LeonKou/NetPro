using Serilog;
using MQTTnet.AspNetCore;

Environment.SetEnvironmentVariable("ASPNETCORE_HOSTINGSTARTUPASSEMBLIES", "NetPro.Startup");
var host = Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) => ApolloClientHelper.ApolloConfig(hostingContext, config, args))
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureKestrel(options =>
                    {
                        //同时开启多个端口监听
                        //options.ListenAnyIP(5000);
                        //options.ListenAnyIP(1883);
                    });
                }).UseSerilog();

host.Build().Run();
