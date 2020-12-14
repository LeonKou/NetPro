
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
				"MaxErrorLimit": 1000000,
				"HitDuration": "2d"  //锁定持续2天
			},
			{
				"Enabled": true,
				"Path": "/api/v1/test/pay/post/account", 
				"MaxSucceedLimit": 100,
				"MaxErrorLimit": 50,
				"HitDuration": "2h"  //锁定持续2小时
			},
			{
				"Enabled": true,
				"Path": "/api/v1/test/pay/post/getname",
				"MaxSucceedLimit": 100,
				"MaxErrorLimit": 50,
				"HitDuration": "10m"  //锁定持续10分钟
			},
			{
				"Enabled": true,
				"Path": "/api/v1/test/pay/post/gatage",
				"MaxSucceedLimit": 100,
				"MaxErrorLimit": 50,
				"HitDuration": "10s"  //锁定持续10秒
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

```
 后期功能:
 全局接口熔断策略:
 20秒以内超过10次请求指定接口错误率超过20% 触发阈值  持续10秒熔断(指标都是可配置)
 全局接口限制策略:
 同ip1小时内错误数大于10次触发阈值(指标都是可配置)
 同ip1小时内请求数大于20次触发阈值(指标都是可配置)

```

