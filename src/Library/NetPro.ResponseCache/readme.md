
## ResponseCache使用
 [![NuGet](https://img.shields.io/nuget/v/NetPro.ResponseCache.svg)](https://nuget.org/packages/NetPro.ResponseCache)

支持对响应缓存，同样的查询条件在缓存时间内直接返回之前的响应接口，降低响应时间

### 使用

#### appsetting.json 

```json
"ResponseCacheOption": {
"Enabled": true,//默认启用
"Cluster":false,
"Duration": 5,//响应持续时间
"IgnoreVaryByQueryKeys": [ "sign,timestamp" ]
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
    app.UseShareRequestBody();//共享body流，UseShareRequestBody必须在UsePostResponseCache中间件之上，而且必须启用，否则响应缓存失效
    app.UsePostResponseCache();//启动Post全局响应缓存,建议不使用全局Post缓存,改用特新方式[PostResponseCacheAttribute],除非在查询条件繁多情况下为了快速组装条件用Post提交条件方式进行查询可使用全局缓存,
    app.UseGetResponseCaching();//启动Get全局响应缓存,也可直接使用[ResponseCache]特性对指定接口启用Get响应缓存
}
```

注意:优先级低于使用 [PostResponseCache(Duration = 10, Order = 1)]

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

````csharp 
Post特性方式启用响应缓存配置

/// <summary>
/// 获取一个查询
/// </summary>
/// <param name="gg"></param>
/// <returns></returns>
[HttpPost]
[Route("pay/post")]
[ProducesResponseType(200)]
[ProducesResponseType(200, Type = typeof(XXXAo))]
[PostResponseCache(Duration = 10,ResponseMode =ResponseMode.Error,Message="请求频繁,稍后再试")]//响应缓存10秒
public IActionResult Cache([FromBody]XXXInput gg)
{
    return BadRequest(new Result { Data = gg.Age, Code = 11 });
}
```

响应StatusCode为1001时为报错模式触发响应缓存