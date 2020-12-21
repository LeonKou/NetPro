<p align="center">
  <img  src="docs/images/netpro2.png">
</p>

# NetPro
![.NET Core](https://github.com/LeonKou/NetPro/workflows/.NET%20Core/badge.svg)  [![NuGet](https://img.shields.io/nuget/v/NetPro.Web.Api.svg)](https://nuget.org/packages/NetPro.Web.Api)


### 🕰️ 项目请参照 

* 👉[*master* branch](https://github.com/LeonKou/NetPro)

## 简要
NetPro项目封装常用组件和初始配置，为快速开发webapi,守护进程,windwos服务提供基础模板,支持.NetCore3.1，支持.Net5 Preview

#### 主要组件：

`FreeSql`,`Autofac` , `Automapper`,`apollo`,`App.Metrics`,

`CsRedisCore`,`StackExchange.Redis`,`Serilog`,

`MiniProfiler`,`FluentValidation`,`IdGen`,

`MongoDb`,`Dapper`,`RedLock.Net`,

`Sentry`,`RabbitMQ.Client`,`SkyAPM`,

`Swagger`,`WebApiClient.Core`,

`TimeZoneConverter`,`healthcheck`

`exceptionless`

### 使用
###### NetPro.Web.Api组件打包封装了其他所有组件，"开箱即用"，各组件已发布Nuget包，也可单独使用，建议直接使用NetPro.Web.Api省去各种初始化与避免配置有误导致的问题

##### 包含的内置组件


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

具体参考sample/Leon.XXX.Api（分层)项目

* webapi项目引用 `NetPro.Web.Api` [![NuGet](https://img.shields.io/nuget/v/NetPro.Web.Api.svg)](https://nuget.org/packages/NetPro.Web.Api)  引用最新nuget即可

Package Manager方式: `Install-Package NetPro.Web.Api -Version 3.1.2`

.NET CLI 方式: `dotnet add package NetPro.Web.Api --version 3.1.2`

PackageReference:`<PackageReference Include="NetPro.Web.Api" Version="3.1.2" />`

.NET CLI 方式: `paket add NetPro.Web.Api --version 3.1.2`

*  修改`Program.cs`

```csharp

public class Program
{
/// <summary>
/// 
/// </summary>
/// <param name="args"></param>
public static void Main(string[] args)
{
	CreateHostBuilder(args).Build().Run();
}

/// <summary>
/// 
/// </summary>
/// <param name="args"></param>
/// <returns></returns>
public static IHostBuilder CreateHostBuilder(string[] args) =>
	Host.CreateDefaultBuilder(args)
	.UseServiceProviderFactory(new AutofacServiceProviderFactory())
	.ConfigureAppConfiguration((hostingContext, config) => ApolloClientHelper.ApolloConfi	(hostingContext, config, args))
	.ConfigureWebHostDefaults(webBuilder =>
	{
		webBuilder.UseStartup<Startup>();
	}).UseSerilog();
	}
```

* 修改 `Startup.cs`

```csharp

public class Startup
{
 #region Fields

 private readonly IConfiguration _configuration;
 private readonly IWebHostEnvironment _webHostEnvironment;
 private IEngine _engine;
 private NetProOtion _NetProOtion;

 #endregion

 #region Ctor

 /// <summary>
 /// 
 /// </summary>
 /// <param name="configuration"></param>
 /// <param name="webHostEnvironment"></param>
 public Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
 {
 	_configuration = configuration;
 	_webHostEnvironment = webHostEnvironment;
 }

 #endregion

 // This method gets called by the runtime. Use this method to add services to the  container.
 /// <summary>
 /// 
 /// </summary>
 /// <param name="services"></param>
 public void ConfigureServices(IServiceCollection services)
 {
 	(_engine, _NetProOtion) = services.ConfigureApplicationServices(_configuration, _webHostEnvironment);
 }

 /// <summary>
 /// 
 /// </summary>
 /// <param name="builder"></param>
 public void ConfigureContainer(ContainerBuilder builder)
 {
 	_engine.RegisterDependencies(builder, _NetProOtion);
 }
 
 // This method gets called by the runtime. Use this method to configure the HTTP request  pipeline.
 /// <summary>
 /// 
 /// </summary>
 /// <param name="app"></param>
 /// <param name="env"></param>
 public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
 {
 	app.ConfigureRequestPipeline();
 }
}
```

* 为了Startup文件干净清爽，建议创建`ApiStartup.cs`文件

此文件继承`INetProStartup`接口，提供了microsoft原生依赖注入能力，所有组件注入放于此 ，Startup.cs将不接受组件注入

* 修改`appsettings.json` 文件

```json

{	
	//数据库ORM建议使用FreeSql，为了便于灵活选择使用适合自己的ORM，框架已剔除内置的NetPro.Dapper
	//apollo配置
	"Apollo": {
		"Enabled": false,
		"AppId": "Leon",
		"MetaServer": "http://192.168.56.98:7078",
		"Cluster": "default",
		"Namespaces": "AppSetting,MicroServicesEndpoint",
		"RefreshInterval": 300000,
		"LocalCacheDir": "apollo/data"
	},
	//响应缓存配置，建议不大于3秒
	"ResponseCacheOption": {
		"Enabled": true,
		"Duration": 3,
		"IgnoreVaryQuery": [ "sign", "timestamp" ]
	},
	//日志配置
	"Serilog": {
		"Using": [ "Serilog.Sinks.Console", "Serilog.Sinks.Async", "Serilog.Sinks.File" ],
		"MinimumLevel": {
			"Default": "Information",
			"Override": {
				"Microsoft": "Debug",
				"System": "Debug",
				"System.Net.Http.HttpClient": "Debug"
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

	"AllowedHosts": "*",
	//框架核心配置
	"NetProOption": {
		"ProjectPrefix": "Leon",
		"ProjectSuffix": "",
		"UseResponseCompression": false,
		"ThreadMinCount": 5,
		"ApplicationName": "",
		"RequestWarningThreshold": 5
	},
	//接口签名防篡改配置
	"VerifySignOption": {		
		"Enable": true,
		"IsDarkTheme":true,
		"IsDebug": false,
		"IsForce": false, //是否强制签名
		"Scheme": "attribute", //attribute;global
		"ExpireSeconds": 60,
		"CommonParameters": {
			"TimestampName": "timestamp",
			"AppIdName": "appid",
			"SignName": "sign"
		},
		"AppSecret": {
			"AppId": {
				"sadfsdf": "sdfsfd"
			}
		},
		"IgnoreRoute": [ "api/ignore/", "" ]
	},
	//swagger配置
	"SwaggerOption": {
		"Enable": true,
		"IsDarkTheme":true,//Swagger黑色主题
		"MiniProfilerEnabled": false,
		"XmlComments": [ "", "" ],
		"RoutePrefix": "swagger",
		"Description": "this is swagger for netcore",
		"Title": "Demo swagger",
		"Version": "first version",
		"TermsOfService": "netcore.com",
		"Contact": {
			"Email": "swagger@netcore.com",
			"Name": "swagger",
			"Url": "swagger@netcore.com"
		},
		"License": {
			"Name": "",
			"Url": ""
		},
		"Headers": [ //swagger默认公共头参数
			{
				"Name": "User",
				"Description": "用户"
			}
		], 
		"Query": [ //swagger默认url公共参数
			{
				"Name": "sign",
				"Description": "签名"
			},
			{
				"Name": "timestamp",
				"Description": "客户端时间戳"
			}
		]
	},
	//中间件健康检查配置
	"HealthChecksUI": {
		"HealthChecks": [
			{
				"Name": "HealthList",
				"Uri": "/health"
			}
		],
		"Webhooks": [],
		"EvaluationTimeOnSeconds": 3600, //检查周期，单位秒
		"MinimumSecondsBetweenFailureNotifications": 60
	},

	"Hosting": {
		"ForwardedHttpHeader": "",
		"UseHttpClusterHttps": false,
		"UseHttpXForwardedProto": false
	},
	//redis配置
	"RedisCacheOption": {
		"Enabled": true,
		"RedisComponent": 1,
		"Password": "netpro",
		"IsSsl": false,
		"Preheat": 20,
		"Cluster": true, //集群模式
		"ConnectionTimeout": 20,
		"Endpoints": [
			{
				"Port": 6379,
				"Host": "192.168.7.66"
			}
		],
		"Database": 0,
		"DefaultCustomKey": "NetPro:",//key前缀
		"PoolSize": 50
	},
	//跨服务访问配置
	"MicroServicesEndpoint": {
		"Example": "http://localhost:5000",
		"Baidu": ""
	},
	//mongodb配置
	"MongoDbOptions": {
		"Enabled": false,
		"ConnectionString": null,
		"Database": -1
	},
	//rabbitmq配置
	"RabbitMq": {
		"HostName": "127.0.0.1",
		"Port": "5672",
		"UserName": "guest",
		"Password": "guest"
	},
	"RabbitMqExchange": {
		"Type": "direct",
		"Durable": true,
		"AutoDelete": false,
		"DeadLetterExchange": "default.dlx.exchange",
		"RequeueFailedMessages": true,
		"Queues": [
			{
				"Name": "myqueue",
				"RoutingKeys": [ "routing.key" ]
			}
		]
	}
}


```

* Controller使用

`Controller`继承`ApiControllerBase`抽象类提供统一响应和简化其他操作，如果不需要默认提供的响应格式也可直接继承ControllerBase

```csharp

	/// <summary>
	///
	/// </summary>
	[Route("api/v1/[controller]")]
	public class WeatherForecastController : ApiControllerBase
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

###### 以下后缀结尾的类将自动DI注入

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

```json

          ____  _____        _   _______
        |_   \|_   _|      / |_|_   __ \
          |   \ | |  .---.`| |-' | |__) |_ .--.   .--.
          | |\ \| | / /__\\| |   |  ___/[ `/'`\]/ .'`\ \
         _| |_\   |_| \__.,| |, _| |_    | |    | \__. |
        |_____|\____|'.__.'\__/|_____|  [___]    '.__.'


[20:20:34 Development] dotnet process id:349824
服务注入顺序：
+-------+------------------------+-------------------------------------------------------+
| Order | StartUpName            | Path                                                  |
+-------+------------------------+-------------------------------------------------------+
| 0     | ErrorHandlerStartup    | NetPro.Web.Core.Infrastructure.ErrorHandlerStartup    |
+-------+------------------------+-------------------------------------------------------+
| 100   | NetProCommonStartup    | NetPro.Web.Core.Infrastructure.NetProCommonStartup    |
+-------+------------------------+-------------------------------------------------------+
| 100   | NetProRateLimitStartup | NetPro.Web.Core.Infrastructure.NetProRateLimitStartup |
+-------+------------------------+-------------------------------------------------------+
| 105   | ShareBodyStartup101    | NetPro.Web.Core.Infrastructure.ShareBodyStartup101    |
+-------+------------------------+-------------------------------------------------------+
| 110   | SignStartup            | NetPro.Web.Core.Infrastructure.SignStartup            |
+-------+------------------------+-------------------------------------------------------+
| 115   | ResponseCacheStartup   | NetPro.Web.Core.Infrastructure.ResponseCacheStartup   |
+-------+------------------------+-------------------------------------------------------+
| 120   | NetProApiStartup       | NetPro.Web.Api.NetProApiStartup                       |
+-------+------------------------+-------------------------------------------------------+
| 500   | AuthenticationStartup  | NetPro.Web.Core.Infrastructure.AuthenticationStartup  |
+-------+------------------------+-------------------------------------------------------+
| 900   | ApiStartup             | Leon.XXX.Api.ApiStartup                               |
+-------+------------------------+-------------------------------------------------------+
| 900   | XXXApiProxyStartup     | Leon.XXX.Proxy.XXXApiProxyStartup                     |
+-------+------------------------+-------------------------------------------------------+
| 1000  | NetProCoreStartup      | NetPro.Web.Core.Infrastructure.NetProCoreStartup      |
+-------+------------------------+-------------------------------------------------------+
| 2000  | ApiProxyStartup        | NetPro.Web.Api.ApiProxyStartup                        |
+-------+------------------------+-------------------------------------------------------+

核心数为：8--默认线程最小为：40--Available:32767
[20:20:51 DBG] Hosting starting
[20:20:51 DBG] Failed to locate the development https certificate at 'null'.
[20:20:51 INF] Now listening on: http://localhost:5001
[20:20:51 DBG] Loaded hosting startup assembly Leon.XXX.Api
[20:20:51 INF] Application started. Press Ctrl+C to shut down.
[20:20:51 INF] Hosting environment: Development
[20:20:51 INF] Content root path: G:\vsFile\netproFile\NetPro\src\sample\Leon.XXX.Api
[20:20:51 DBG] Hosting started

```

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

环境信息 [/env](/env)、

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
# ...

