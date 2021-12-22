
## RedisManager使用

支持 StackExchange.Redis，支持分布式锁。
redisjson参考：[redisjson](https://blog.alumdb.org/using-redisjson/)

### appsetting.json

```json

"RedisConfiguration": {
		"Enabled": true,
		"KeyPrefix": "net:",
		"Hosts": [
			{
				"Host": "185.56.167.565",
				"Port": 6379
			}
		],
		"ConnectTimeout": 1000,
		"Database": 0,
		"Password": "666666",
		"ssl": false
	}
```

### 使用
#### 启用redis服务
```csharp
  public void ConfigureServices(IServiceCollection services)
  {
    //新增redis缓存注入
   services.AddStackExchangeRedisExtensions<SystemTextJsonSerializer>(configuration);
  }
```

#### 构造函数注入

```csharp
public class WeatherForecastController : ControllerBase
{
        private readonly IRedisDatabase _redisDatabase;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="redisManager"></param>
        public WeatherForecastController(
            IRedisDatabase redisDatabase)
        {
            _redisDatabase_ = redisDatabase;
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
 var result = _redisDatabase_.GetOrCreate<string>("1",func: ()=>//获取key的值，没有找到则执行委托并将委托返回的值插入redis缓存
 {
     return "1";
 },TimeSpan.FromeSecond(66));


//异步方法
  var resultAsync = await _redisDatabase_.GetOrCreateAsync<string>("1",func: async()=>//获取key的值，没有找到则执行委托并将委托返回的值插入redis缓存
 {
     return await Task.FromResult("1") ;
 });
```
