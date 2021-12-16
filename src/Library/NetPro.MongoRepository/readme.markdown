
## Mongodb使用
 [![NuGet](https://img.shields.io/nuget/v/NetPro.Authentication.svg)](https://nuget.org/packages/NetPro.Authentication)

对Mongodb的封装，简化使用

### 使用

#### appsetting.json 

```json
"MongoDbOptions": {
		"Enabled": false,
		"ConnectionString": null,
		"Database": -1
	},

```
#### 启用服务
```csharp
IConfiguration Configuration;

public void ConfigureServices(IServiceCollection services)
{
      //MongoDb 连接配置文件
       services.AddMongoDb(Configuration);
}
```

### 使用说明


#### 更新中...

