
## Core使用
 [![NuGet](https://img.shields.io/nuget/v/NetPro.Core.svg)](https://nuget.org/packages/NetPro.Core)

框架的基础支撑，主要包含依赖注入，startup生命周期管理等核心逻辑

### 使用

#### appsetting.json 

```json
基础配置

"NetProOption": {     
      "UseResponseCompression": false,//是否启用响应压缩      
      "RequestWarningThreshold": 5, //请求时长的警告临界值
      "RoutePrefix":"api" //全局路由前缀
	},

```

#### 常用功能

##### 静态方式获取对象实例

``` csharp
EngineContext.Current.Resolve<对象类型>();
```
##### 依赖注入 

除了使用官方原生依赖注入外，支持通过接口和正则模式注入

*接口方式注入*

1、`ITransientDependency`

2、`ISingletonDependency`

3、`IScopedDependency`

*正则批量注入*
```C#
services.BatchInjection("^XXX.", "Service$"); //批量注入以XXX前缀的程序集，Service结尾的类
```
##### Apollo配置中心

Program.cs
```csharp

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
                        //options.Limits.MaxRequestBodySize = null;// 消除异常 Unexpected end of request content.
                    });
                });

host.Build().Run();

```