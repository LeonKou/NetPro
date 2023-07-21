
## NetPro.Pulsar使用


### 使用
[![NuGet](https://img.shields.io/nuget/v/NetPro.Pulsar.svg)](https://nuget.org/packages/NetPro.Pulsar)

#### 初始化Pulsar服务（代码方式）
```csharp
public void ConfigureServices(IServiceCollection services, IConfiguration configuration = null, ITypeFinder typeFinder = null)
        {
            //pulsar注入
            services.AddPulsarClient(configuration);
        }
```

json待补充
```json

```

