using Microsoft.AspNetCore.Server.Kestrel.Core;
using Serilog;

var host = Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) => ApolloClientHelper.ApolloConfig(hostingContext, config, args))
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureKestrel(options =>
                    {
                        //监听端口，可配多个使用
                        //options.ListenLocalhost(11837, o => o.Protocols =
                        //    HttpProtocols.Http2);
                    });
                }).UseSerilog();

host.Build().Run();
