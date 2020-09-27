
## ANetPro.Analysic使用
 [![NuGet](https://img.shields.io/nuget/v/NetPro.Analysic.svg)](https://nuget.org/packages/NetPro.Analysic)

精确到同ip下对请求流量的精确控制，可控制1天 或者1小时内的错误数正确数

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
				"Path": "/api/sdk/v2/accounts/action/register/account", //注册账号
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

