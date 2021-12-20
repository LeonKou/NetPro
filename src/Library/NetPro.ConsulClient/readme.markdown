
## NetPro.Proxy使用
 [![NuGet](https://img.shields.io/nuget/v/NetPro.ConsulClient.svg)](https://nuget.org/packages/NetPro.ConsulClient)

Consul 客户端

### 使用


#### appsetting.json 

- 增加以下配置节点
```json
"ConsulOption": {
		"HealthPath": "/HealthCheck",//可空不填，默认HealthCheck
		"ServiceName": "xxx.api",//可空不填，取运行时程序集名称
		"EndPoint": "http://localhost:8500" consul服务地址
	}
```
如果基于NetPro.WebApi的项目，只需要以上配置即可，如不是基于NetPro.WebApi的项目，需手动按一下初始化组件
#### 启用服务
``` csharp
public void ConfigureServices(IServiceCollection services, IConfiguration configuration = null)
        {
            services.AddConsul(configuration);
        }

public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseConsul();
        }
```
