var host = Host.CreateDefaultBuilder(args)
               .ConfigureAppConfiguration((context, config) =>
               {
                   //config.AddAgileConfig();
               })
               .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup("NetPro.Startup"));
host.Build().Run();
