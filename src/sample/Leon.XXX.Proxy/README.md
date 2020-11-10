# Proxy层


### 🕰️ 说明


#### 简要
Proxy层主要负责跨服务调用

#### 建议结构：

```
++Leon.XXX.Proxy
	XXXProxy.cs	//强制Proxy结尾以实现移动注入
```

#### 引用关系
依赖 XXX.Repository层

#### 使用

在appsettings.json文件`MicroServicesEndpoint`节点中进行配置Host后使用

```csharp
public interface IExampleProxy : IHttpApi
	{
		[HttpGet("")]
		ITask<dynamic> GetAsync(string account);

		[HttpPost("api/v1/NetProgoods/list")]
		ITask<dynamic> GetGoodsList(int appid, string appVersion);

		// POST api/user 
		[HttpPost("api/user")]
		ITask<dynamic> AddAsync([FormContent] dynamic user);
	}
```

