<p align="center">
  <img  src="docs/images/netpro.png">
</p>

# NetPro
![.NET Core](https://github.com/LeonKou/NetPro/workflows/.NET%20Core/badge.svg)  [![NuGet](https://img.shields.io/nuget/v/NetPro.Web.Api.svg)](https://nuget.org/packages/NetPro.Web.Api)

# 👉[点击进入主页](https://leonkou.github.io/NetPro/) 

### 🕰️ 项目请参照 

* 👉[*master* branch](https://github.com/LeonKou/NetPro)

## 简要
NetPro项目封装常用组件和初始配置，为快速开发webapi,守护进程,windwos服务提供基础模板

#### 主要组件：

`Autofac` , `Automapper`,`apollo`,`App.Metrics`,

`CsRedisCore`,`StackExchange.Redis`,`Serilog`,

`MiniProfiler`,`FluentValidation`,`IdGen`,

`MongoDb`,`Dapper`,`RedLock.Net`,

`Sentry`,`RabbitMQ.Client`,`SkyAPM`,

`Swagger`,`WebApiClient.JIT`,

`TimeZoneConverter`,`healthcheck`
`exceptionless`

### 使用

具体参考sample/Leon.XXXV2.Api项目

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
				.ConfigureAppConfiguration((hostingContext, config) => ApolloClientHelper.ApolloConfig(hostingContext, config, args))
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

		// This method gets called by the runtime. Use this method to add services to the container.
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

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
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
	"Apollo": {
		"Enabled": false,
		"AppId": "Leon",
		"MetaServer": "http://192.168.56.98:7078",
		"Cluster": "default",
		"Namespaces": "AppSetting,MicroServicesEndpoint",
		"RefreshInterval": 300000,
		"LocalCacheDir": "apollo/data"
	},

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
			"Application": "GameSdk"
		}
	},

	"AllowedHosts": "*",

	"NetProOption": {
		"ProjectPrefix": "Leon", //Project prefix name,,for example："Leon.User.Api"'s prefix is Leon
		"ProjectSuffix": "",
		"DisplayFullErrorStack": false,
		"StaticFilesCacheControl": "Cache-Control",
		"UseResponseCompression": false,
		"ThreadMinCount": 5,
		"APMEnabled": false,
		"PermissionEnabled": false,
		"MiniProfilerEnabled": false,
		"ApplicationName": "",
		"SuperRole": "admin",
		"RequestWarningThreshold": 5,
		"AppType": 1,
		"ErrorUrl": "www.Leon.com",
		"Permission": "url",
		"LoginUrl": "",
		"PageNotFoundUrl": "",
		"IsDebug": true,
		"CorsOrigins": "false",
		"EnabledHealthCheck": true,
		"ConnectionStrings": {
			"DefaultConnection": "Server=192.168.57.66;Port=3306;Database=netprodb;charset=utf8;user=netpro;password=netpro;",
			"ServerIdConnection": {
				"1": "Server=192.168.57.68;Port=3306;Database=netprodb1;charset=utf8;user=netpro;password=netpro;"
			}
		}
	},

	"VerifySignOption": {
		"Enable": true,
		"IsDebug": true,
		"Scheme": "attribute",
		"ExpireSeconds": 60,
		"CommonParameters": {
			"TimestampName": "timestamp",
			"AppIdName": "appid",
			"SignName": "sign"
		},
		"AppSecret": {
			"AppId": {

			}
		}
	},

	"SwaggerOption": {
		"Enable": true,
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
		"Headers": [ "User" ] //设置swagger默认头参数
	},

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

	"RedisCacheOption": {
		"Enabled": true,
		"RedisComponent": 2,
		"Password": "netpro",
		"IsSsl": false,
		"Preheat": 20,
		"Cluster": true, //集群模式
		"ConnectionTimeout": 20,
		"Endpoints": [
			{
				"Port": 6665,
				"Host": "192.168.66.33"
			},
			{
				"Port": 6666,
				"Host": "192.168.66.66"
			}
		],
		"Database": 0,
		"DefaultCustomKey": "",
		"PoolSize": 50
	},

	"MicroServicesEndpoint": {
		"Example": "http://localhost:5000",
		"XXX": ""
	},

	"MongoDbOptions": {
		"Enabled": false,
		"ConnectionString": "",
		"Database": -1
	}

	,
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
  _   _      _ _           _   _      _
 | | | | ___| | | ___     | \ | | ___| |_ ___ ___  _ __ ___
 | |_| |/ _ \ | |/ _ \    |  \| |/ _ \ __/ __/ _ \| '__/ _ \
 |  _  |  __/ | | (_) |  _| |\  |  __/ || (_| (_) | | |  __/
 |_| |_|\___|_|_|\___/  (_)_| \_|\___|\__\___\___/|_|  \___|

[13:00:00 Development] dotnet process id:15120
[13:00:02 DBG] Hosting starting
[13:00:02 DBG] Failed to locate the development https certificate at 'null'.
[13:00:02 INF] Now listening on: http://localhost:5001
[13:00:02 DBG] Loaded hosting startup assembly Leon.XXX.Api
[13:00:02 INF] Application started. Press Ctrl+C to shut down.
[13:00:02 INF] Hosting environment: Development
[13:00:02 INF] Content root path: F:\自己代码库\NetPro\src\sample\Leon.XXX.Api
[13:00:02 DBG] Hosting started
[13:00:03 DBG] Connection id "0HM0SM9PEGG6G" accepted.
[13:00:03 DBG] Connection id "0HM0SM9PEGG6H" accepted.
[13:00:03 DBG] Connection id "0HM0SM9PEGG6H" started.
[13:00:03 DBG] Connection id "0HM0SM9PEGG6G" started.
[13:00:03 DBG] Connection id "0HM0SM9PEGG6H" received FIN.
[13:00:03 DBG] Connection id "0HM0SM9PEGG6G" received FIN.
[13:00:03 DBG] Connection id "0HM0SM9PEGG6G" sending FIN because: "The client closed the connection."
[13:00:03 DBG] Connection id "0HM0SM9PEGG6H" sending FIN because: "The client closed the connection."
[13:00:03 DBG] Connection id "0HM0SM9PEGG6G" disconnecting.
[13:00:03 DBG] Connection id "0HM0SM9PEGG6H" disconnecting.
[13:00:03 DBG] Connection id "0HM0SM9PEGG6H" stopped.
[13:00:03 DBG] Connection id "0HM0SM9PEGG6G" stopped.
[13:00:03 DBG] Connection id "0HM0SM9PEGG6I" accepted.
[13:00:03 DBG] Connection id "0HM0SM9PEGG6I" started.
[13:00:03 DBG] Connection id "0HM0SM9PEGG6J" accepted.
[13:00:03 DBG] Connection id "0HM0SM9PEGG6J" started.
[13:00:03 INF] Request starting HTTP/1.1 GET http://localhost:5001/swagger/index.html
[13:00:03 DBG] Wildcard detected, all requests with hosts will be allowed.
[13:00:03 DBG] The request path /swagger/index.html does not match an existing file
[13:00:03 DBG] Connection id "0HM0SM9PEGG6I" completed keep alive response.
[13:00:03 INF] Request finished in 113.6636ms 200 text/html;charset=utf-8
[13:00:03 INF] Request starting HTTP/1.1 GET http://localhost:5001/docs/v1/docs.json
[13:00:03 DBG] The request path /docs/v1/docs.json does not match an existing file
[13:00:03 DBG] Connection id "0HM0SM9PEGG6I" completed keep alive response.
[13:00:03 INF] Request finished in 113.9995ms 200 application/json;charset=utf-8
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

[文档持续更新中...]
