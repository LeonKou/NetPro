
# NetPro.Grpc Server端使用
 [![NuGet](https://img.shields.io/nuget/v/NetPro.Grpc.svg)](https://nuget.org/packages/NetPro.Grpc)

Grpc Server辅助组件

## 使用

### 初始化服务

#### 没有使用NetPro.Core的方式，需手动显示注入

```csharp
  public void ConfigureServices(IServiceCollection services)
   {
       services.AddGrpc();
   }

  public void Configure(IApplicationBuilder application)
   {
    application.UseRouting();

    application.UseEndpoints(endpoints =>
    {
         GrpcServiceExtension.AddGrpcServices(endpoints,new string[] {$"{Assembly.GetEntryAssembly().GetName().Name}" });
    });
   }
```

#### 在有NetPro.Core的基础上的使用

直接引用NetPro.Grpc nuget包无需其他配置即可

