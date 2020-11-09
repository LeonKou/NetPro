
## RedisManager使用

支持CsRedis，支持分布式锁

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
  public void ConfigureServices(IServiceCollection services)
  {
    //新增redis缓存注入
    services.AddCsRedis<NetPro.CsRedis.SystemTextJsonSerializer>(Configuration);
  }
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
 ///获取或者创建缓存 
 /// localExpiredTime参数大于0并且小于expiredTime数据将缓存到本地内存
 /// </summary>
 /// <typeparam name="T"></typeparam>
 /// <param name="key"></param>
 /// <param name="func"></param>
 /// <param name="expiredTime">redis过期时间，默认不过期</param>
 /// <param name="localExpiredTime">本地过期时间，0 默认本地不缓存单位秒</param>
 /// <returns></returns>
 T GetOrSet<T>(string key, Func<T> func = null, TimeSpan? expiredTime = null, int localExpiredTime = 0);
```

#### 方法中调用
```csharp

//同步方法
 var result = _redisManager.GetOrCreate<string>("1",func: ()=>//获取key的值，没有找到则执行委托并将委托返回的值插入redis缓存
 {
     return "1";
 },TimeSpan.FromeSecond(66));


//异步方法
  var resultAsync = await _redisManager.GetOrCreateAsync<string>("1",func: async()=>//获取key的值，没有找到则执行委托并将委托返回的值插入redis缓存
 {
     return await Task.FromResult("1") ;
 });
```
