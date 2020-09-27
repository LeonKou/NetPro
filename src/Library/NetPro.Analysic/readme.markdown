
## NetPro.Analysic使用
 [![NuGet](https://img.shields.io/nuget/v/NetPro.Analysic.svg)](https://nuget.org/packages/NetPro.Analysic)

请求分析控制，可精细配置同ip一定时间的错误数和正确数，超过阈值拦截熔断访问

### 使用

#### appsetting.json 

```json
	"RequestAnalysisOption": {
		"Enabled": true, //是否开启流量分析
		"PolicyOption": [
			{
				"Enabled": true,
				"Path": "/api/v1/test/pay/post",
				"MaxSucceedLimit": 1000000, //当前path地址同ip一天内最大访问次数
				"MaxErrorLimit": 1000000
			},
			{
				"Enabled": true,
				"Path": "/api/test/add", 
				"MaxSucceedLimit": 100,
				"MaxErrorLimit": 50
			}
		]
	}

```
#### 启用服务
```csharp
public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
{
    services.AddRequestAnalysic();
}

public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    app.UseRequestAnalysis();
}
```

