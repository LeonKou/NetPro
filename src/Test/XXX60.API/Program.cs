using Serilog;

Environment.SetEnvironmentVariable("ASPNETCORE_HOSTINGSTARTUPASSEMBLIES", "NetPro.Startup");//;NetPro.ConsulClient");

var host = Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    Serilog.Log.Logger = new Serilog.LoggerConfiguration()
                     .ReadFrom.Configuration(config.Build())
                     .CreateLogger(); //根据需要安装Serilog，并打开注释；相关serilog nuget包已在程序入口所在cspro工程文件中
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    //webBuilder.UseSerilog();
                    webBuilder.ConfigureKestrel(options =>
                    {
                        //options.Limits.MaxRequestBodySize = null;// 消除异常 Unexpected end of request content.
                    });
                });

host.Build().Run();
