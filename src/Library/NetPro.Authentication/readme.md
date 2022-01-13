
## Authentication使用
 [![NuGet](https://img.shields.io/nuget/v/NetPro.Authentication.svg)](https://nuget.org/packages/NetPro.Authentication)

对Microsoft.AspNetCore.Authentication认证组件的封装，简化使用

### 使用

#### appsetting.json 

```json
"AuthenticationOption": {
  "Enabled": true,
  "Secret": "6666666666666666", //最少配置16位
  "Issuer": "NetPro.com",
  "AccessTokenExpired": 6, //单位小时
  "RefreshTokenExpired": 168 //单位小时
}

```
#### 启用服务
```csharp
public void ConfigureServices(IServiceCollection services)
{
      services.AddNetProAuthentication(configuration);
}

public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
     app.UseNetProAuthentication();
}
```

如增加了以下环境变量配置，可省略以上初始化代码，内部将自动执行以上初始化

```
 "ASPNETCORE_HOSTINGSTARTUPASSEMBLIES": "NetPro.Startup"
```

### 使用说明

``` csharp
[Authorize]
/// <summary>
/// 方法或者类加[Authorize]特性，此接口或者此控制器将启用JWT认证才可访问，具体参考示例
/// </summary>
public void Method()

```

``` csharp
[AllowAnonymous]
/// <summary>
/// 方法或者类加[Authorize]特性，此接口或者此控制器将强制匿名访问，具体参考示例
/// </summary>
public void Method()

```
###### 备注
如认证接口数量大于匿名接口数量，建议接口全局默认认证访问,可省去频繁[Authorize]

``` csharp
 app.UseEndpoints(endpoints =>
      {
          endpoints.MapControllers().RequireAuthorization();
      });

```

