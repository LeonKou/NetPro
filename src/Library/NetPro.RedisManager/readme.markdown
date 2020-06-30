
## RedisManager使用

[![NuGet](https://img.shields.io/nuget/v/NetPro.RedisManager.svg)](https://nuget.org/packages/NetPro.RedisManager)

同时支持StackExchangeRedis和CsRedis

### appsetting.json

```json

"RedisCacheOption": {
   "Enabled": true,//是否开启
   "RedisComponent": -1,//-1：NullRedis;1:CSRedis;2:StackExchangeRedis
   "Password": "netpro",
   "IsSsl": false,
   "Preheat": 20,//预热
   "Cluster": true, //集群模式
   "ConnectionTimeout": 20,
   "Endpoints": [
   	{
   	 "Port": 6665,
   	 "Host": "192.168.66.33"
   	},
   	{
   	 "Port": 6666,
   	 "Host": "192.168.66.66"
   	}
   ],
   "Database": 0,
   "DefaultCustomKey": "",
   "PoolSize": 50
},

```

### 使用
#### 启用redis服务
```csharp
//新增redis缓存注入
if (configuration.GetValue<bool>("RedisCacheOption:Enabled",false))//是否开启redis
    services.AddRedisManager(configuration);
```

#### 构造函数注入

```csharp
public class WeatherForecastController : ControllerBase
{
        private readonly IRedisManager _redisManager;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="redisManager"></param>
        public WeatherForecastController(
            IRedisManager redisManager)
        {
            _redisManager = redisManager;
        }
}

```

#### 方法中调用
```csharp

 var dd = _redisManager.GetOrCreate<string>("1",func: ()=>//获取key的值，没有则执行委托并将委托返回的值插入redis
 {
     return "1";
 });
```
