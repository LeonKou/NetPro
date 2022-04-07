using Serilog;

var host = Host.CreateDefaultBuilder(args)
               .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup("NetPro.Startup"))
               .UseSerilog();//如需serilog日志功能请取消UseSerilog注释

host.Build().Run();
