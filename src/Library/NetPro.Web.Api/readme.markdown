
## NetPro.Web.Api使用
 [![NuGet](https://img.shields.io/nuget/v/NetPro.Web.Api.svg)](https://nuget.org/packages/NetPro.Swagger)

封装了基本asp.netcore webapi工程需要的基本功能

### 安装

* Package Manager方式:
 `Install-Package NetPro.Web.Api -Version 3.1.2`

* .NET CLI 方式: 
`dotnet add package NetPro.Web.Api --version 3.1.2`

* PackageReference:
`<PackageReference Include="NetPro.Web.Api" Version="3.1.2" />`

* .NET CLI 方式:
 `paket add NetPro.Web.Api --version 3.1.2`

#### 注意

引用程序集会自动注入相关服务，无需再进行Service.Add..();

### 使用

#### appsetting.json

```json


```

```csharp

public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
	
    app.UseExceptionHandler();   //对应ErrorHandlerStartup0
    
	// your code
	...//此区间请求管道上的Order取值 范围0-100

    app.UseStaticFiles();        //对应StaticFilesStartup100

	// your code
	...//此区间请求管道上的Order取值 范围101-200

    app.UseRouting();            //对应RoutingStartup200

    // your code
	...//此区间请求管道上的Order取值 范围200-1000

    app.UseEndpoints(endpoints =>//对应EndpointsStartup1000
    {
        endpoints.MapRazorPages();
    });
}
```