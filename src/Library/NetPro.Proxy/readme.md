
## NetPro.Proxy使用
 [![NuGet](https://img.shields.io/nuget/v/NetPro.ResponseCache.svg)](https://nuget.org/packages/NetPro.ResponseCache)

远程调用组件

### 使用

- 如果已添加环境变量ASPNETCORE_HOSTINGSTARTUPASSEMBLIES=NetPro.Satrtup      启用自动初始化，添加appsetting.json 配置即可

#### appsetting.json 

- 增加以下配置节点
```json
"NetProProxyOption": {
		"AssemblyPattern": "^XXX.*.Proxy$",//批量注入程序集的正则,此处表示将XXX开头，Proxy结尾的程序集中使用了NetProProxy功能的接口批量注入
		"InterfacePattern": "^I.*.Proxy$", //I开头，Proxy结尾的接口
        "IExampleProxy": "http://localhost:5000",//名称要与具体定义的接口名称一致,例如此项对应的接口定义为 public interface IExampleProxy{}
		"IBaiduProxy": "http://baidu.com"
	}
```
#### 启用服务

如果没添加ASPNETCORE_HOSTINGSTARTUPASSEMBLIES=NetPro.Satrtup环境变量，按以下方式注入服务，并添加上一条appsetting.json 节点配置即可
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

复制以下代码放在请求方法顶部以特性方式使用，可实现方法的请求与响应的拦截处理，如需个性化处理，以此作为模板稍作改动即可

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
             var uri= context.HttpContext.RequestMessage.RequestUri;
                Console.WriteLine($"request uri is：{uri}");
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
