## NetPro.FreeRedis使用

支持FreeRedis，支持分布式锁,支持多个Redis server

### appsetting.json


```json
"RedisCacheOption": {
  "ConnectionString": [
    {
      "Key": "1", //连接串key别名，唯一
      "Value": "127.0.0.1:6379,password=123,defaultDatabase=0,ssl=false,prefix=key前辍" //别名key对应的连接串
    }
  ]
}
```

### 使用

#### NetPro项目启用Redis服务

引用此nuget包，并且配置了`RedisCacheOption`，即可自动实现各种注册配置，无需再关心初始化等操作



#### 非NetPro项目启用Redis服务
- 默认注入方式，读取本地配置
```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddFreeRedis(configuration);
}
```
- 自定义连接
```C#
public void ConfigureServices(IServiceCollection services, IConfiguration configuration = null, ITypeFinder typeFinder = null)
{
    // 基于NetPro.Web.Api的程序，FreeRedis支持自动根据配置文件初始化，如需覆盖默认初始化逻辑可在此重新初始化。
    services.AddFreeRedis(configuration, GetConnectionString);
}

public List<ConnectionString> GetConnectionString(IServiceProvider serviceProvider)
{
    return new List<ConnectionString>
    {
        new ConnectionString
        {
            Key = "2",
            Value = "127.0.0.1:6379,password=123,defaultDatabase=0,ssl=false,prefix=key前辍"
        }
    };
}
```

### 构造函数注入

```csharp
public class WeatherForecastController : ControllerBase
{
        private readonly RedisClient _redisClient;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="redisIdleBus"></param>
        public WeatherForecastController(IdleBus<RedisClient> redisIdleBus)
        {
            // FreeRedis原生对象，以最原生方式调用，支持操作多个redis库；_redisIdleBus.Get("别名")
            _redisClient = redisIdleBus.Get("2");
        }
}
```

### 方法中调用
```csharp

// Get
_redisClient.Get<string>(key);

// Publish
_redisClient.Publish(channel, message);

// 分布式锁
using var distributeLock = _redisClient.Lock(lockKey, timeoutSeconds, autoDelay);
```
