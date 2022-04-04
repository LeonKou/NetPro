
## RedisManager使用

支持CsRedis，支持分布式锁,支持多个Redis server

### appsetting.json


```json

"RedisCacheOption": {
		"Enabled": true,
		"ConnectionString": [//如配置远程获取此节点可删除
			{
				"Key": "1", //连接串key别名，唯一,用来物理隔离
				"Value": "127.0.0.1:6379,password=123,defaultDatabase=0,poolsize=10,preheat=20,ssl=false,writeBuffer=10240,prefix=key前辍,testcluster=false,idleTimeout=10" //别名key对应的连接串
			}
		]
}

```

### 使用
#### 启用redis服务
- 默认注入方式，读取本地配置
```csharp
  public void ConfigureServices(IServiceCollection services)
  {
    //新增redis缓存注入
    services.AddCsRedis().Build<SystemTextJsonSerializer>(configuration);//连接串默认读取配置文件的RedisCacheOption节点

  }
```
- 自定义连接
```C#
public void ConfigureServices(IServiceCollection services, IConfiguration configuration = null, ITypeFinder typeFinder = null)
{
    //基于NetPro.Web.Api的程序，CsRedis支持自动根据配置文件初始化，如需覆盖默认初始化逻辑可在此重新初始化。
    services.AddCsRedis<SystemTextJsonSerializer>(configuration, GetConnectionString);
}

public List<ConnectionString> GetConnectionString(IServiceProvider serviceProvider)
{
    return new List<ConnectionString>
    {
        new ConnectionString
        {
            Key = "2",
            Value = "192.168.100.187:6379,password=,defaultDatabase=0,poolsize=10,preheat=20,ssl=false,writeBuffer=10240,prefix=key前辍,testcluster=false,idleTimeout=10"
        }
    };
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
   /// <param name="expiredTime"></param>
   /// <param name="localExpiredTime">本地过期时间</param>
 T GetOrSet<T>(string key, Func<T> func = null, TimeSpan? expiredTime = null, int localExpiredTime = 0, string dbKey = default);
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
