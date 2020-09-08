
## ShareRequestData使用
 [![NuGet](https://img.shields.io/nuget/v/NetPro.ShareRequestData.svg)](https://nuget.org/packages/NetPro.ShareRequestData)

共享请求何响应中的body，避免各中间件重复解析body流。
特别注意:ResponseCacheData生命周期为scope,不建议single中构造函数注入ResponseCacheData使用

### 使用

#### 启用服务
```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddShareRequestBody();//用户共享存放中间件读取的body
}

public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    //应放于所有中间价最上层
      application.UseShareRequestBody();
}
