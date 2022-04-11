using Serilog;

Environment.SetEnvironmentVariable("ASPNETCORE_HOSTINGSTARTUPASSEMBLIES", "NetPro.Startup");
var host = Host.CreateDefaultBuilder(args)
               .ConfigureWebHostDefaults(webBuilder => { }// webBuilder
                                                          //.UseSetting(WebHostDefaults.HostingStartupAssembliesKey, "NetPro.Startup;NetPro.Prometheus")
                                                          //.UseStartup("NetPro.Startup")//以上方式只能取其一，环境变量更灵活
               ).UseSerilog();//如需serilog日志功能请取消UseSerilog注释；

host.Build().Run();
