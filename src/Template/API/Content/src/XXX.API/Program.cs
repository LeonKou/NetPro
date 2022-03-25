using Serilog;

Environment.SetEnvironmentVariable("ASPNETCORE_HOSTINGSTARTUPASSEMBLIES", "NetPro.Startup");
var host = Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(webBuilder => { }).UseSerilog();//如需serilog日志功能请取消UseSerilog注释
host.Build().Run();
