
# NetPro.Serilog
 [![NuGet](https://img.shields.io/nuget/v/NetPro.Serilog.svg)](https://nuget.org/packages/NetPro.Serilog)

## 使用

### 依赖NetPro.Web.Api组件的模式

1、安装引用

[NetPro.Serilog](https://nuget.org/packages/NetPro.Serilog) [![NuGet](https://img.shields.io/nuget/v/NetPro.Serilog.svg)](https://nuget.org/packages/NetPro.Serilog)(已包含文件写入能力)

[Serilog.Sinks.Async](https://nuget.org/packages/Serilog.Sinks.Async) [![NuGet](https://img.shields.io/nuget/v/Serilog.Sinks.Async.svg)](https://nuget.org/packages/Serilog.Sinks.Async)(异步写入能力)

如需要写入Elasticsearch，安装以下Nuget包即可

[Serilog.Sinks.ElasticSearch](https://nuget.org/packages/Serilog.Sinks.ElasticSearch) [![NuGet](https://img.shields.io/nuget/v/Serilog.Sinks.ElasticSearch.svg)](https://nuget.org/packages/Serilog.Sinks.ElasticSearch)

2、增加配置
```json
//日志相关配置 Information  Error
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft": "Debug",
      "Microsoft.Hosting.Lifetime": "Debug"
    }
  },
  "Serilog": {
    "MinimumLevel": {
      "Default": "Debug",
      "Override": {
        "Default": "Information",
        "Microsoft": "Information",
        "System": "Information",
        "System.Net.Http.HttpClient": "Information",
        "Microsoft.Hosting.Lifetime": "Information"
      }
    },
    "WriteTo:Async": {
      "Name": "Async",
      "Args": {
        "configure": [
          {
            "Name": "Console"
          }
          ,{
            "Name": "File",
            "Args": {
              "path": "Logs/.txt",
              "rollingInterval": "Day",
              "retainedFileCountLimit": "31", //Number of retained files by default 31
              "retainedFileTimeLimit": "10.00:00:00", //Deletes files older than 10 days
              "outputTemplate": "{Timestamp:o} [{Level:u3}] ({Application}/{MachineName}/{ThreadId}) {Message}{NewLine}{Exception}"
            }
          },          
          {
            "Name": "Elasticsearch",
            "Args": {
              "nodeUris": "http://elasticsearch.com:9200",
              "indexFormat": "xxxapi-{0:yyyy.MM}",
              "autoRegisterTemplate": true,
              "connectionGlobalHeaders": "Authorization=ApiKey XzVPaDFIOEJNNUduR1FGWFdiWlc6Sms5ajFUNHJTWDJ5VzEtOG4zSllsdw=="
            }
          }
        ]
      }
    },
    "Enrich": [ "FromLogContext", "WithMachineName", "WithThreadId" ],
    "Properties": {
      "Application": "XXX"
    }
  }
}
```
### 纯净模式下不依赖NetPro.Web.Api模式

1、安装引用 [![NuGet](https://img.shields.io/nuget/v/NetPro.Serilog.svg)](https://nuget.org/packages/NetPro.Serilog) Nuget包

2、 注册初始化

```csharp
public void Configure(IApplicationBuilder application, IWebHostEnvironment env)
{
    application.UseSerilogRequestLogging();
}

public void ConfigureServices(IServiceCollection services, IConfiguration configuration = null)
{
            Log.Logger = new LoggerConfiguration()
                                .ReadFrom.Configuration(configuration)
                                .CreateLogger();

            // Reference to https://github.com/serilog/serilog-aspnetcore/blob/dev/src/Serilog.AspNetCore/SerilogWebHostBuilderExtensions.cs
            services.AddSingleton<ILoggerFactory>(services => new SerilogLoggerFactory());

            // Registered to provide two services...
            var diagnosticContext = new DiagnosticContext(null);

            // Consumed by e.g. middleware
            services.AddSingleton(diagnosticContext);

            // Consumed by user code
            services.AddSingleton<IDiagnosticContext>(diagnosticContext);
}
```

3、 配置

```json
//日志相关配置 Information  Error
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft": "Debug",
      "Microsoft.Hosting.Lifetime": "Debug"
    }
  },
  "Serilog": {
    "MinimumLevel": {
      "Default": "Debug",
      "Override": {
        "Default": "Information",
        "Microsoft": "Information",
        "System": "Information",
        "System.Net.Http.HttpClient": "Information",
        "Microsoft.Hosting.Lifetime": "Information"
      }
    },
    "WriteTo:Async": {
      "Name": "Async",
      "Args": {
        "configure": [
          {
            "Name": "Console"
          }
          ,{
            "Name": "File",
            "Args": {
              "path": "Logs/.txt",
              "rollingInterval": "Day",
              "retainedFileCountLimit": "31", //Number of retained files by default 31
              "retainedFileTimeLimit": "10.00:00:00", //Deletes files older than 10 days
              "outputTemplate": "{Timestamp:o} [{Level:u3}] ({Application}/{MachineName}/{ThreadId}) {Message}{NewLine}{Exception}"
            }
          },          
          {
            "Name": "Elasticsearch",
            "Args": {
              "nodeUris": "http://elasticsearch.com:9200",
              "indexFormat": "xxxapi-{0:yyyy.MM}",
              "autoRegisterTemplate": true,
              "connectionGlobalHeaders": "Authorization=ApiKey XzVPaDFIOEJNNUduR1FGWFdiWlc6Sms5ajFUNHJTWDJ5VzEtOG4zSllsdw=="
            }
          }
        ]
      }
    },
    "Enrich": [ "FromLogContext", "WithMachineName", "WithThreadId" ],
    "Properties": {
      "Application": "XXX"
    }
  }
}
```