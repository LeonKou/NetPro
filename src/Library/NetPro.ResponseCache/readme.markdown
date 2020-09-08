
## ResponseCache使用
 [![NuGet](https://img.shields.io/nuget/v/NetPro.ResponseCache.svg)](https://nuget.org/packages/NetPro.ResponseCache)

支持对响应缓存，同样的查询条件在缓存时间内直接返回之前的响应接口，降低响应时间

### 使用

#### appsetting.json 

```json
"ResponseCacheOption": {
"Enabled": "true",
"Expired": 5,//响应过期时间
"ExcluedQuery": [ "sign,timestamp" ]
}
```
#### 启用服务
```csharp
public void ConfigureServices(IServiceCollection services)
{
    //响应缓存
    services.AddResponseCachingExtension();
}

public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
     app.UsePostResponseCache();//响应缓存
}
```
#### 使用
```csharp 

Get请求缓存配置

 /// <summary>
 /// 根据account检测账户
 /// VaryByQueryKeys中的值将参与缓存，如以下接口，只要account参数值是同一个，无论sign参数如何变化始终将命中缓存
 /// </summary>
 /// <param name="account">用户名</param>
 /// <param name="sign">签名，每次请求都将变化</param>
 /// <returns></returns>
[HttpGet("action/exist")]
[ProducesResponseType(200)]
[ResponseCache(Duration = 10, VaryByQueryKeys = new[] { "account"})]
public async Task IsExistAccount(string account,string sign)
```