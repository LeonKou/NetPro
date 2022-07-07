var host = Host.CreateDefaultBuilder(args)
               .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup("NetPro.Startup"));
host.Build().Run();
