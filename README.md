<p align="center">
  <img height="150" src="docs/images/netpro.png">
</p>

# NetPro

### 🕰️ 项目请参照 

* [*master* branch](https://github.com/LeonKou/NetPro)

## 简要
NetPro项目封装常用组件和初始配置，为快速开发webapi,守护进程,windwos服务提供基础模板

#### 主要组件：

`Autofac` * `Automapper`,`apollo`,`App.Metrics`,

`CsRedisCore`,`StackExchange.Redis`,`Serilog`,

`MiniProfiler`,`FluentValidation`,`IdGen`,

`MongoDb`,`Dapper`,`RedLock.Net`,

`Sentry`,`RabbitMQ.Client`,`SkyAPM`,

`Swagger`,`WebApiClient.JIT`,`TimeZoneConverter`

### 使用
具体参考sample/Leon.XXXV2.Api项目

* webapi项目引用 `NetPro.Web.Api`

Package Manager方式: `Install-Package NetPro.Web.Api -Version 1.0.0`

.NET CLI 方式: `dotnet add package NetPro.Web.Api --version 1.0.0`

PackageReference:`<PackageReference Include="NetPro.Web.Api" Version="1.0.0" />`

.NET CLI 方式: `paket add NetPro.Web.Api --version 1.0.0`

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

* 增加 `ApiStartup.cs`文件

此文件继承`INetProStartup`接口，提供了microsoft原生依赖注入能力，所有组件注入放于此 ，Startup.cs将不接受组件注入

* 修改`appsettings.json` 文件

```json

{
 "Apollo": {
    "AppId": "NetPro",
    "MetaServer": "http://189.16.85.62:9080",
    "Cluster": "default",
    "Namespaces": "AppSetting,MicroServicesEndpoint",
    "RefreshInterval": 300000,
    "LocalCacheDir": "apollo/data"
  },
"Logging": {
"LogLevel": {
    "Default": "Information",
    "Microsoft": "Information",
    "Microsoft.Hosting.Lifetime": "Information"
    }
},
"AllowedHosts": "*",
"NetProOption": {
      "DisplayFullErrorStack": false,
      "StaticFilesCacheControl": "Cache-Control",
      "UseResponseCompression": false,
      "RedisCacheEnabled": false,
      "ThreadMinCount": 5,
      "DistributedLogEnabled": false,
      "SerilogSinks": null,//"console,debug,file",
      "SerilogMinimumLevel": 2,
      "RedisCacheComponent": 2,
      "APMEnabled": false,
      "PermissionEnabled": false,
      "MiniProfilerEnabled": false,
      "ApplicationName": "",
      "SuperRole": "admin",
      "RequestWarningThreshold": 5,
      "AppType": 1,
      "ErrorUrl": "www.netpro.com",
      "Permission": "url",
      "LoginUrl": "",
      "PageNotFoundUrl": "",
      "IsDebug": false,
      "CorsOrigins": "false",
      "ConnectionStrings": {
       "DefaultConnection": "156.16.183.168;Port=3563;Database=center;charset=utf8;user=yutyu;password=LKPL%ylLdLNjn%Au;",
       "ServerIdConnection": {
      "1": "Server=",
      "2": "Server="
   }
  },
  "SwaggerDoc": {
   "Title": "",
   "Description": "",
   "EnableUI": true}
	},
	"HealthChecksUI": {
		"HealthChecks": [
   {
    "Name": "HealthList",
    "Uri": "/health"
			}
		],
		"Webhooks": [],
		"EvaluationTimeOnSeconds": 3600,
		"MinimumSecondsBetweenFailureNotifications": 60
	},
	"Hosting": {
		"ForwardedHttpHeader": "",
		"UseHttpClusterHttps": false,
		"UseHttpXForwardedProto": false
	},
	"RedisCacheOption": {
		"RedisComponent": 1,
		"Password": "rtyrr",
		"IsSsl": false,
		"ConnectionTimeout": 20,
		"Endpoints": [
			{
				"Port": 6379,
				"Host": "192.168.231.133"
			}
		],
		"Database": 0,
		"DefaultCustomKey": "",
		"PoolSize": 50
	},
	"MicroServicesEndpoint": {
		"Example": "http://localhost:5000",
		"XXX": ""
	}
}


```

* Controller使用

`Controller`继承`ApiControllerBase`抽象类提供统一响应和简化其他操作

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


[11:16:52 Development] dotnet process id:25820
配置： NetProOption:{
  "DisplayFullErrorStack": false,
  "StaticFilesCacheControl": "Cache-Control",
  "UseResponseCompression": false,
  "RedisCacheEnabled": false,
  "ThreadMinCount": 5,
  "DistributedLogEnabled": false,
  "SerilogSinks": "console,debug,file",
  "RedisCacheComponent": 2,
  "APMEnabled": false,
  "PermissionEnabled": false,
  "MiniProfilerEnabled": false,
  "ConnectionStrings": {
      "DefaultConnection": "192.168.56.89;Port=40036;Database=leon;charset=utf8;user=leon;password=*******;",
      "ServerIdConnection": {
        "1": "Server=",
        "2": "Server="//...
      }
    },
  "ApplicationName": "",
  "SuperRole": "admin",
  "RequestWarningThreshold": 5,
  "SwaggerDoc": {
    "Title": "title",
    "Description": "this is Description",
    "EnableUI": true
  },
  "AppType": 1,
  "ErrorUrl": "www.netpro.com",
  "Permission": "url",
  "LoginUrl": "",
  "PageNotFoundUrl": "",
  "IsDebug": false,
  "CorsOrigins": "false"
}
核心数为：6--默认线程最小为：30--Available:32767
[11:16:53 DBG] Hosting starting
  health:/health
  env:/env
  info:/info
[11:16:53 DBG] Failed to locate the development https certificate at 'null'.
[11:16:53 INF] Now listening on: http://localhost:5001
```

Swagger地址：[/docs/index.html](ip:port/docs/index.html)

健康检查地址 [/health](health)

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