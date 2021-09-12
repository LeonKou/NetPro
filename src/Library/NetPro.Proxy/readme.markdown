
## NetPro.Proxy使用
 [![NuGet](https://img.shields.io/nuget/v/NetPro.ResponseCache.svg)](https://nuget.org/packages/NetPro.ResponseCache)

远程调用组件

### 使用

#### appsetting.json 

- 通过NetPro.WebApi nuget的使用方式
只需要增加以下配置节点即可
```json
"MicroServicesEndpoint": {
		"Assembly": null,//程序集完整名称，批量注入
		"Example": "http://localhost:5000",//命名规则为请求接口去除结尾Proxy与I关键字，例如此项对应请求接口IExampleProxy
		"Baidu": "http://baidu.com"
	}
```
#### 启用服务
- 通过直接使用NetPro.Proxy nuget的方式
按以下方式注入服务，并添加上一条appsetting.json 节点配置即可
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
 public interface IExampleProxy //命名对应appsetting.json 中的Example节点
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

##### 使用过滤

```csharp

    /// <summary>
    /// 过滤器
    /// </summary>
    public class WebApiClientFilter : ApiFilterAttribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>

        public override Task OnRequestAsync(ApiRequestContext context)
        {
            //请求开始前做的拦截
            return Task.CompletedTask;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task OnResponseAsync(ApiResponseContext context)
        {
            //对于响应做的拦截
            Console.WriteLine($"HasResult：{context.ResultStatus}");
            Console.WriteLine($"context.Result：{context.Result}");

            var resultString = context.HttpContext.ResponseMessage.Content.ReadAsStringAsync().Result;
            Console.WriteLine($"ReadAsStringAsync()：   {resultString}");
            Console.WriteLine($"StatusCode：   {context.HttpContext.ResponseMessage.StatusCode}");

            return Task.CompletedTask;
        }
    }
```
