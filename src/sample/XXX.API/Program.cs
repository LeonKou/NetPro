//支持环境变量方式叠加生效 ASPNETCORE_HOSTINGSTARTUPASSEMBLIES
var host = Host.CreateDefaultBuilder(args)
    //.UseAgileConfig()
    .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup("NetPro.Startup"));
host.Build().Run();
