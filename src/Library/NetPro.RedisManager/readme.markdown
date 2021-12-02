
## RedisManager使用

同时支持StackExchangeRedis和CsRedis，支持分布式锁
**不推荐使用此库，Redis请使用指定客户端实现的 NetPro.CsRedis、NetPro.StackExchange.Redis或者原生CsRedis**

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

```csharp
方法说明
/// <summary>
/// 获取缓存，没有则新增缓存
/// 不过期或者过期时间时间大于一小时，数据将缓存到本地内存
/// 过期时间等于-1(永不过期)的缓存无法覆盖更新，建议删除后新增
/// </summary>
/// <typeparam name="T"></typeparam>
/// <param name="key">缓存健值，建议key最少三段以:分割便于的管理</param>
/// <param name="func">缓存不存在要执行的委托</param>
/// <param name="expiredTime">过期时间，单位秒</param>
/// <returns></returns>
T GetOrCreate<T>(string key, Func<T> func = null, int expiredTime -1);
```

#### 方法中调用
```csharp

//同步方法
 var result = _redisManager.GetOrCreate<string>("1",func: ()=>//获取key的值，没有找到则执行委托并将委托返回的值插入redis缓存
 {
     return "1";
 },500);


//异步方法
  var resultAsync = await _redisManager.GetOrCreateAsync<string>("1",func: ()=>//获取key的值，没有找到则执行委托并将委托返回的值插入redis缓存
 {
     return "1";
 });
```
