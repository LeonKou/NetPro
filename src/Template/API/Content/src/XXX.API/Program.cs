using Microsoft.AspNetCore.Server.Kestrel.Core;
using Serilog;

var host = Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) => ApolloClientHelper.ApolloConfig(hostingContext, config, args))
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureKestrel(options =>
                    {
                        options.ListenLocalhost(5001, o => o.Protocols =
                            HttpProtocols.Http2);

                        // ADDED THIS LINE to fix the problem
                        options.ListenLocalhost(11837, o => o.Protocols =
                            HttpProtocols.Http1);
                    });
                }).UseSerilog();

host.Build().Run();
