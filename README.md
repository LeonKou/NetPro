<p align="center">
  <img  src="docs/images/netpro2.png">
</p>

# NetPro
![.NET Core](https://github.com/LeonKou/NetPro/workflows/.NET%20Core/badge.svg)  [![NuGet](https://img.shields.io/nuget/v/NetPro.Web.Api.svg)](https://nuget.org/packages/NetPro.Web.Api)


### 🕰️ 项目请参照 

* 👉[*dev_6.0* branch](https://github.com/LeonKou/NetPro/tree/dev_3.1)

## 简要

.NetpPro 是一个基于.NetCore的插件化开发脚手架，支持以搭“积木”方式实现业务模块的开发，支持`net6.0`，核心包`NetPro.Startup` 只有16k即可支撑整个框架以插件方式进行开发。NetPro项目在基于核心包`NetPro.Startup`基础上提供了其他周边常用中间件，其核心封装逻辑也是尽可能的暴露原生方法，不对开发人员产生过多不必要的学习成本。
基于`NetPro.Startup`的有两个关键插件包：
- `NetPro.Web.Api` 用于开发webapi项目
- `NetPro.Grpc` 用于开发grpc项目

其他的所有NetPro中间件都是可插拔，按需引用，每个组件都相对轻量，没有强关联NetPro，即使非.NetPro框架也可使用。如果基于`NetPro.Web.Api`或 `NetPro.Grpc`引用的NetPro中间件，支持引用即`自动执行`初始化逻辑。


### 架构图

<p align="center">
  <img  src="docs/images/netproinfo.png">
</p>


#### 提供的中间件包含不限于以下：

`FreeSql`,`Autofac` , `Automapper`,`apollo`,`App.Metrics`,

`CsRedisCore`,`StackExchange.Redis`,`Serilog`,

`MiniProfiler`,`FluentValidation`,`IdGen`,

`MongoDb`,`Dapper`,`RedLock.Net`,

`Sentry`,`RabbitMQ.Client`,`SkyAPM`,

`Swagger`,`WebApiClient.Core`,

`TimeZoneConverter`,`healthcheck`

`exceptionless`


### 使用
##### 各组件已发布Nuget包，可单独使用，如需插件化和自动执行初始化请使用`NetPro.Web.Api`或者`NetPro.Grpc`

##### 支持的组件


- [![NuGet](https://img.shields.io/nuget/v/NetPro.Web.Core.svg)](https://nuget.org/packages/NetPro.Web.Core) [NetPro.Web.Core](https://github.com/LeonKou/NetPro.Web.Core)（辅助NetPro.Web.Api) 
- [![NuGet](https://img.shields.io/nuget/v/NetPro.Web.Api.svg)](https://nuget.org/packages/NetPro.Web.Api) [NetPro.Web.Api](https://github.com/LeonKou/NetPro.Web.Api) （包含所有常用组件）

- [![NuGet](https://img.shields.io/nuget/v/NetPro.TypeFinder.svg)](https://nuget.org/packages/NetPro.TypeFinder) [NetPro.TypeFinder](https://github.com/LeonKou/NetPro.TypeFinder) （dll检索，反射）

- [![NuGet](https://img.shields.io/nuget/v/NetPro.Utility.svg)](https://nuget.org/packages/NetPro.Utility) [NetPro.Utility](https://github.com/LeonKou/NetPro.Utility) （包含常用帮助类）

- [![NuGet](https://img.shields.io/nuget/v/NetPro.Authentication.svg)](https://nuget.org/packages/NetPro.Authentication) [NetPro.Authentication](https://github.com/LeonKou/NetPro.Authentication) （包含常用帮助类）

- [![NuGet](https://img.shields.io/nuget/v/NetPro.Checker.svg)](https://nuget.org/packages/NetPro.Checker) [NetPro.Checker](https://github.com/LeonKou/NetPro.Checker) （组件健康检查）

- [![NuGet](https://img.shields.io/nuget/v/NetPro.Dapper.svg)](https://nuget.org/packages/NetPro.Dapper) [NetPro.Dapper](https://github.com/LeonKou/NetPro.Dapper) （dapper封装，建议使用FreeSql)


- [![NuGet](https://img.shields.io/nuget/v/NetPro.Log.svg)](https://nuget.org/packages/NetPro.Log ) [NetPro.Log ](https://github.com/LeonKou/NetPro.Log ) （日志,废弃，已集成于NetPro.WebApi）

- [![NuGet](https://img.shields.io/nuget/v/NetPro.MongoDb.svg)](https://nuget.org/packages/NetPro.MongoDb ) [NetPro.MongoDb ](https://github.com/LeonKou/NetPro.MongoDb ) （mongodbi）

- [![NuGet](https://img.shields.io/nuget/v/NetPro.RabbitMQ.svg)](https://nuget.org/packages/NetPro.RabbitMQ ) [NetPro.RabbitMQ ](https://github.com/LeonKou/NetPro.RabbitMQ ) （rabbitmq组件的封装，特性方式消费消息）

- [![NuGet](https://img.shields.io/nuget/v/NetPro.RedisManager.svg)](https://nuget.org/packages/NetPro.RedisManager ) [NetPro.RedisManager ](https://github.com/LeonKou/NetPro.RedisManager ) （Redis组件，包含CsRedis，StackExchangeRedis，分布式锁）

- [![NuGet](https://img.shields.io/nuget/v/NetPro.Swagger.svg)](https://nuget.org/packages/NetPro.Swagger ) [NetPro.Swagger ](https://github.com/LeonKou/NetPro.Swagger ) （Swagger，包含认证，文件上传，公共参，个性主题）

- [![NuGet](https://img.shields.io/nuget/v/NetPro.EFCore.svg)](https://nuget.org/packages/NetPro.EFCore ) [NetPro.EFCore ](https://github.com/LeonKou/NetPro.EFCore ) （EFCore批量注入DbSet,建议使用FreeSql）

- [![NuGet](https://img.shields.io/nuget/v/NetPro.Sign.svg)](https://nuget.org/packages/NetPro.Sign ) [NetPro.Sign ](https://github.com/LeonKou/NetPro.Sign ) （签名）

- [![NuGet](https://img.shields.io/nuget/v/NetPro.ResponseCache.svg)](https://nuget.org/packages/NetPro.ResponseCache ) [NetPro.ResponseCache ](https://github.com/LeonKou/NetPro.ResponseCache ) （响应缓存）

- [![NuGet](https://img.shields.io/nuget/v/NetPro.NetProShareRequestBody.svg)](https://nuget.org/packages/NetPro.NetProShareRequestBody ) [NetPro.NetProShareRequestBody ](https://github.com/LeonKou/NetPro.NetProShareRequestBody ) （共享Body流，辅助其他中间件与过滤器）

- [![NuGet](https://img.shields.io/nuget/v/NetPro.Analysic.svg)](https://nuget.org/packages/NetPro.Analysic) [NetPro.Analysic](https://github.com/LeonKou/NetPro.Analysic)(请求分析控制，可精细配置同ip一定时间的错误数和正确数，超过阈值拦截熔断访问)

---

具体参考 NetPro\src\Template\API\Content（插模块插件)项目

 webapi项目引用 `NetPro.Web.Api` [![NuGet](https://img.shields.io/nuget/v/NetPro.Web.Api.svg)](https://nuget.org/packages/NetPro.Web.Api)  引用最新nuget即可

Package Manager方式: `Install-Package NetPro.Web.Api -Version 3.1.2`

.NET CLI 方式: `dotnet add package NetPro.Web.Api --version 3.1.2`

PackageReference:`<PackageReference Include="NetPro.Web.Api" Version="3.1.2" />`

.NET CLI 方式: `paket add NetPro.Web.Api --version 3.1.2`

---

## 通过脚手架创建项目(推荐)

### 1、安装netproapi[![NuGet](https://img.shields.io/nuget/v/netproapi.svg)](https://nuget.org/packages/netproapi)脚手架
执行以下命令安装脚手架
```
dotnet new -i netproapi::* 
```

### 2、使用脚手架创建项目

在指定的项目文件夹中执行以下命令
```
dotnet new netproapi -n 项目名称
```
例如当前项目为IAM
```
dotnet new netproapi -n IAM
```
执行以上命令将自动创建WebApi代码解决方案

## 手动创建项目

*  修改`Program.cs`

```csharp
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Serilog;

var host = Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) => ApolloClientHelper.ApolloConfig(hostingContext, config, args))
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureKestrel(options =>
                    {
                    });
                }).UseSerilog();

host.Build().Run();

```

* 增加 `EndpointsStartup.cs`文件配置
可加多个继承了INetProStartup接口的启动类来控制启动顺序和初始化
此文件继承`INetProStartup`接口，提供了microsoft原生依赖注入能力，所有组件注入放于此 ，Startup.cs将不接受组件注入

```csharp
// <auto-generated>
//  UseEndpoints   
// </auto-generated>

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NetPro.Web.Api
{
    /// <summary>
    /// Endpoints 
    /// </summary>
    public class EndpointsStartup : INetProStartup
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <param name="typeFinder"></param>
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration, ITypeFinder typeFinder)
        {

        }

        /// <summary>
        /// Endpoints请求管道;
        /// Order执行顺序保证在RoutingStartup（200）之后即可
        /// </summary>
        /// <param name="application"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder application, IWebHostEnvironment env)
        {
            application.UseEndpoints(s =>
            {
                s.MapControllers();
            });
        }

        /// <summary>
        /// UseEndpoints can be executed after RoutingStartup
        /// </summary>
        public double Order { get; set; } = 1000;
    }
}

```

* 修改`appsettings.json` 文件

```json
{
	"TypeFinderOption": {
		"MountePath": ""//插件挂载路径
	},
	"Serilog": {
		"Using": [ "Serilog.Sinks.Console", "Serilog.Sinks.Async", "Serilog.Sinks.File" ],
		"MinimumLevel": {
			"Default": "Information",
			"Override": {
				"Default": "Error",
				"Microsoft": "Debug",
				"System": "Debug",
				"System.Net.Http.HttpClient": "Debug",
				"Microsoft.Hosting.Lifetime": "Information"
			}
		},
		"WriteTo:Async": {
			"Name": "Async",
			"Args": {
				"configure": [
					{ "Name": "Console" }
				]
			}
		},
		"Enrich": [ "FromLogContext", "WithMachineName", "WithThreadId" ],
		"Properties": {
			"Application": "Netpro"
		}
	},

	//**********以上日志***************

	"AllowedHosts": "*",

	"SwaggerOption": {
		"Enabled": true
	}
}


```

* 启动前设置环境变量开启增强启动
```csharp
Environment.SetEnvironmentVariable("ASPNETCORE_HOSTINGSTARTUPASSEMBLIES", "NetPro.Startup")
```



* Controller使用

`Controller`继承原生`ControllerBase`即可，使用也参考官方原生接口开发

```csharp

	/// <summary>
	///
	/// </summary>
	[Route("api/v1/[controller]")]
	public class WeatherForecastController : ControllerBase
	{
		private readonly ILogger _logger;
		private IExampleProxy _userApi { get; set; }

		public WeatherForecastController(ILogger logger
			 ,IExampleProxy userApi)
		{
			_logger = logger;
			_userApi = userApi;
		}
	}
```
#### 约定


* `Service` 业务相关
* `Repository` 数据仓储相关(需要直接在Service或Controller中直接操作数据库可无需使用此后缀)
* `Proxy` 代理请求相关（请求远程接口使用）
* `Aggregate` 聚合相关，当Service 或者Controller 业务逻辑繁琐复杂可在此聚合后再调用

### 发布

###### 发布自包含应用

```
dotnet publish -r linux-x64 -c release /p:PublishSingleFile=true /p:PublishTrimmed=true
```
###### 依赖CLR运行时应用
```
dotnet publish -r linux-x64 -c release
```

### 运行

开发环境运行后效果如下：

```
 ____  _____        _   _______
|_   \|_   _|      / |_|_   __ \
  |   \ | |  .---.`| |-' | |__) |_ .--.   .--.
  | |\ \| | / /__\\| |   |  ___/[ `/'`\]/ .'`\ \
 _| |_\   |_| \__.,| |, _| |_    | |    | \__. |
|_____|\____|'.__.'\__/|_____|  [___]    '.__.'


[17:40:03] dotnet process id:14520
The enhanced service has started
[17:40:04] loading json files
Service injection sequence：
[17:40:15] apollo已关闭
info: NetProSwaggerServiceExtensions[0]
      NetPro Swagger 已启用
--------------------------------------------------------------------------------------------------------------------------------------
|    Order   |       StartupClassName       |                   Path                   |             Assembly            |  Version  |
--------------------------------------------------------------------------------------------------------------------------------------
| 0          | NetProCoreStartup            | NetPro.Core.Startup.NetProCoreStartup    | NetPro.Core                     |  3.1.13.0 |
--------------------------------------------------------------------------------------------------------------------------------------
| 0          | CheckerStartup               | NetPro.Analysic.CheckerStartup           | NetPro.Checker                  |  3.1.13.0 |
--------------------------------------------------------------------------------------------------------------------------------------
| 0          | ErrorHandlerStartup(default) | NetPro.Web.Api.ErrorHandlerStartup       | NetPro.Web.Api                  |  3.1.13.0 |
--------------------------------------------------------------------------------------------------------------------------------------
| 100        | SwaggerStartup               | NetPro.Swagger.SwaggerStartup            | NetPro.Swagger                  |  3.1.13.0 |
--------------------------------------------------------------------------------------------------------------------------------------
| 100        | StaticFilesStartup(default)  | NetPro.Web.Api.StaticFilesStartup        | NetPro.Web.Api                  |  3.1.13.0 |
--------------------------------------------------------------------------------------------------------------------------------------
| 200        | RoutingStartup(default)      | NetPro.Web.Api.RoutingStartup            | NetPro.Web.Api                  |  3.1.13.0 |
--------------------------------------------------------------------------------------------------------------------------------------
| 300        | CorsStartup                  | NetPro.Web.Api.CorsStartup               | NetPro.Web.Api                  |  3.1.13.0 |
--------------------------------------------------------------------------------------------------------------------------------------
| 400        | ShareBodyStartup             | NetPro.ShareRequestBody.ShareBodyStartup | NetPro.NetProShareRequestBody   |  3.1.13.0 |
--------------------------------------------------------------------------------------------------------------------------------------
| 1000       | EndpointsStartup(default)    | NetPro.Web.Api.EndpointsStartup          | XXX.API (custom)                |  1.0.0.0  |
--------------------------------------------------------------------------------------------------------------------------------------
| 1000       | NetProCsRedisStartup         | NetPro.CsRedis.NetProCsRedisStartup      | NetPro.CsRedis                  |  3.1.13.0 |
--------------------------------------------------------------------------------------------------------------------------------------
| 2147483647 | ApiStartup                   | XXX.API.ApiStartup                       | XXX.API (custom)                |  1.0.0.0  |
--------------------------------------------------------------------------------------------------------------------------------------
| 2147483647 | ApiStartup-1                 | XXX.Plugin.Web.Demo.ApiStartup           | XXX.Plugin.Web.Demo (custom)    |  1.0.0.0  |
--------------------------------------------------------------------------------------------------------------------------------------
| 2147483647 | ApiStartup-2                 | XXX.Plugin.Web.Manager.ApiStartup        | XXX.Plugin.Web.Manager (custom) |  1.0.0.0  |

```
第一次初始化会自动在程序当前目录生成`startup.json`文件可修改对应Order来覆盖各中间件默认执行顺序
```json
{
  "NetProCoreStartup": 0,
  "CheckerStartup": 0,
  "ErrorHandlerStartup": 0,
  "SwaggerStartup": 100,
  "StaticFilesStartup": 100,
  "RoutingStartup": 200,
  "CorsStartup": 300,
  "ShareBodyStartup": 400,
  "EndpointsStartup": 1000,
  "ApiStartup": 2147483647,
  "ApiStartup-1": 2147483647,
  "ApiStartup-2": 2147483647
}

```

### 插件方式开发

可将开发好的dll丢入TypeFinderOption:MountePath 配置的文件路径中，即可自动加载当前dll无需工程文件引用

Swagger地址：[/swagger/index.html](ip:port/docs/index.html)
<p align="center">
  <img  src="docs/images/swagger.jpg">
</p>

健康检查地址 [/health](health)

健康检查面板[/ui](healthdashboard)
<p align="center">
  <img  src="docs/images/checkhealth.jpg">
</p>

应用信息 [/info](/info)

环境信息 [/env](/env)

## 问题汇总

### 1. 如何覆盖系统异常处理

```csharp
var mvcBuilder = services.AddControllers(config =>
   {
    config.Filters.Add(typeof(CustomerExceptionFilter),2);//自定义全局异常过滤器//100是order值，越大越靠后加载
});
```
### ...
## Target

- Microsoft.SourceLink.Github 加入
- 可视化安装卸载组件

# ...

