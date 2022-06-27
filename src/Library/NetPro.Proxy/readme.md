
 # ~~NetPro.Proxy远程调用~~
 [![NuGet](https://img.shields.io/nuget/v/NetPro.ResponseCache.svg)](https://nuget.org/packages/NetPro.Proxy)

此库已**归档**，推荐直接使用原生组件,说明请查阅
[WebApiClientCore使用说明](https://www.cnblogs.com/kewei/p/12939866.html)

归档原因：
原生组件使用已足够方便，没有再次封装意义。

##  最佳使用建议
[WebApiClientCore](https://www.cnblogs.com/kewei/p/12939866.html) 组件已屏蔽了过多细节，使用已足够便捷，推荐按作者说明使用。

接口配置建议以配置文件方式

配置建议按以下标准
```json
"Remoting": {
 "IUserApi": {//接口定义,与代码interface一致
    "HttpHost": "http://www.user.com/",
    "UseParameterPropertyValidate": false,
    "UseReturnValuePropertyValidate": false,
    "JsonSerializeOptions": {
        "IgnoreNullValues": true,
        "WriteIndented": false
        }
     },
 "IAdminApi": {//接口定义，与代码interface一致
    "HttpHost": "http://www.admin.com/",
    "UseParameterPropertyValidate": false,
    "UseReturnValuePropertyValidate": false,
    "JsonSerializeOptions": {
        "IgnoreNullValues": true,
        "WriteIndented": false
        }
     }
}
```
定义远程接口
``` csharp
/// <summary>
/// 记得要实现IHttpApi
/// </summary>
public interface IUserApi : IHttpApi
{ 
    [HttpGet("api/users/{id}")]
    Task<User> GetAsync(string id);
    ...
}

public interface IAdminApi : IHttpApi
{ 
    [HttpGet("api/users/{id}")]
    Task<User> GetAsync(string id);
    ...
}
```
注册
```csharp
     var sectionUser = configuration.GetSection($"Remoting:{nameof(ITaosProxy)}");
     services.AddHttpApi<ITaosProxy>().ConfigureHttpApi(section).ConfigureHttpApi(o =>
      {
          // 符合国情的不标准时间格式，有些接口就是这么要求必须不标准
          o.JsonSerializeOptions.Converters.Add(new JsonDateTimeConverter("yyyy-MM-dd HH:mm:ss"));
      });

     var sectionAdmin = configuration.GetSection($"Remoting:{nameof(IAdminApi)}");
     services.AddHttpApi<IAdminApi>().ConfigureHttpApi(section).ConfigureHttpApi(o =>
      {
          // 符合国情的不标准时间格式，有些接口就是这么要求必须不标准
          o.JsonSerializeOptions.Converters.Add(new JsonDateTimeConverter("yyyy-MM-dd HH:mm:ss"));
      });
```

使用
``` csharp
public class MyService
{
    private readonly IUserApi userApi;
    public MyService(IUserApi userApi)
    {
        this.userApi = userApi;
    }
}
```

## 以下为过时的文档
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
