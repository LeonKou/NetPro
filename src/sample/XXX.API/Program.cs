using Serilog;

Environment.SetEnvironmentVariable("ASPNETCORE_HOSTINGSTARTUPASSEMBLIES", "NetPro.Startup");//;NetPro.ConsulClient");

var host = Host.CreateDefaultBuilder(args)
                //.ConfigureAppConfiguration((hostingContext, config) =>
                //{
                //    ApolloClientHelper.ApolloConfig(hostingContext, config, args);
                //})
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureKestrel(options =>
                    {
                        //options.Limits.MaxRequestBodySize = null;// 消除异常 Unexpected end of request content.
                    });
                }).UseSerilog();//如需serilog日志功能请取消此行注释
host.Build().Run();
