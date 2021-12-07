
## Core使用
 [![NuGet](https://img.shields.io/nuget/v/NetPro.Core.svg)](https://nuget.org/packages/NetPro.Core)

框架的基础支撑，主要包含依赖注入，startup生命周期管理等核心逻辑

### 使用

#### appsetting.json 

```json
基础配置

"NetProOption": {     
      "UseResponseCompression": false,//是否启用响应压缩
      "ThreadMinCount": 5,//最小线程数
      "ApplicationName": "",//应用名称
      "RequestWarningThreshold": 5, //请求时长的警告临界值
      "RoutePrefix":"api" //全局路由前缀
	},

```

#### 常用功能

##### 静态方式获取对象实例

``` csharp
EngineContext.Current.Resolve<对象类型>();
```

##### Apollo配置中心

```csharp
 /// <summary>
 /// 
 /// </summary>
 /// <param name="args"></param>
 /// <returns></returns>
 public static IHostBuilder CreateHostBuilder(string] args) =>
     Host.CreateDefaultBuilder(args)
        .UseServiceProviderFactory(new utofacServiceProviderFactory())
        .ConfigureAppConfiguration((hostingContext, onfig) => ApolloClientHelper.ApolloConfighostingContext, config, args))
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseStartup<Startup>();
        });
```