//using Serilog;

Environment.SetEnvironmentVariable("ASPNETCORE_HOSTINGSTARTUPASSEMBLIES", "NetPro.Startup");

var host = Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    ApolloClientHelper.ApolloConfig(hostingContext, config, args);
                    //Serilog.Log.Logger = new Serilog.LoggerConfiguration()
                    // .ReadFrom.Configuration(config.Build())
                    // .CreateLogger(); //根据需要安装Serilog，并打开注释；相关serilog nuget包已在程序入口所在cspro工程文件中
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {

                    //webBuilder.UseSerilog();
                    webBuilder.ConfigureKestrel(options =>
                    {
                    });
                });

host.Build().Run();
