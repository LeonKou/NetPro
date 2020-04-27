<p align="center">
  <img  src="docs/images/netpro.png">
</p>

# NetPro

### 🕰️ 项目请参照 

* [*master* branch](https://github.com/LeonKou/NetPro)

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
		"MinimumSecondsBetweenFail
