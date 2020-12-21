
## NetPro.Proxy使用
 [![NuGet](https://img.shields.io/nuget/v/NetPro.ResponseCache.svg)](https://nuget.org/packages/NetPro.ResponseCache)

远程调用组件

### 使用

#### appsetting.json 

```json
"MicroServicesEndpoint": {
		"Assembly": null,//程序集完整名称，批量注入
		"Example": "http://localhost:5000",
		"Baidu": "http://baidu.com"
	}
```
#### 启用服务
```csharp
public void ConfigureServices(IServiceCollection services)
{ 
    services.AddFileProcessService();
            var typeFinder = services.BuildServiceProvider().GetRequiredService<ITypeFinder>();
       services.AddHttpProxy(configuration, typeFinder, configuration.GetValue<string>("MicroServicesEndpoint:Assembly", string.Empty));
        
}
```


#### 使用
##### 定义服务
```csharp
 public interface IExampleProxy
    {
        [HttpGet("")]//HttpGet服务
        [WebApiClientFilter]//服务过滤器
        ITask<dynamic> GetAsync([Parameter(Kind.Query)]string account);

        [HttpPost("api/v1/NetProgoods/list")]
        [Timeout(10 * 1000)] // 10s超时
        [WebApiClientFilter]
        ITask<dynamic> GetGoodsList(int appid, string appVersion);

        // POST api/user 
        [HttpPost("api/user")]
        [WebApiClientFilter]
        ITask<dynamic> AddAsync([FormContent] dynamic user);

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="Captcha"></param>
        /// <returns></returns>
        [HttpPost("/api/ldap")]
        [Timeout(10 * 1000)] // 10s超时
        [JsonReturn(Enable = false)]
        [Cache(60 * 1000)]//接口缓存
        [WebApiClientFilter]
        ITask<dynamic> LoginByPwd([Uri] string url, [Parameter(Kind.Query)] string username, string password, string Captcha);
    }
```
##### 服务注入
```csharp 

 public class HttpProxyController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IExampleProxy _exampleProxy;
        //构造函数注入
        public HttpProxyController(
            ILogger<DatabaseCurdController> logger
            IExampleProxy exampleProxy)
        {
            _logger = logger;
            _exampleProxy = exampleProxy;
        }

         [HttpGet("getorcreate")]
        [PostResponseCache(Duration = 2)]
        [ProducesResponseType(200, Type = typeof(ResponseResult))]
        public async Task<IActionResult> GetOrCreateAsync(uint id)
        {
            await _exampleProxy.AddAsync("");//直接使用定义的接口
            return Ok();
        }
    }
```
