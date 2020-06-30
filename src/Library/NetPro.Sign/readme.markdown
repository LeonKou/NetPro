
## 接口签名

[![NuGet](https://img.shields.io/nuget/v/NetPro.Sign.svg)](https://nuget.org/packages/NetPro.Sign)

主要防范请求参数被篡改和增加爬虫难度，支持Attribute和Global 模式

### 接口签名使用
默认为url参数与body参数合并成一个字符串再utf-8编码后进行摘要计算，得到的值转为16进制小写

startup注入

``` csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddVerifySign(s =>
            {
                s.OperationFilter<VerifySignCustomer>();
            });
}
```

```csharp
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
     application.Use(next => context =>
     {       
         //此设置用于其他地方读取Body https://stackoverflow.com/questions/31389781/read-request-body-twice
         context.Request.EnableBuffering();
         return next(context);
     });
}
```

### 自定义摘要算法

```csharp
 public class VerifySignCustomer : IOperationFilter
    {
        private readonly IConfiguration _configuration;

        public VerifySignCustomer(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// 根据appid获取secret
        /// </summary>
        /// <param name="gameId"></param>
        /// <returns></returns>
        public string GetSignSecret(string appid)
        {
            var secret = "1111";
            return secret;
        }

        /// <summary>
        /// 定义摘要算法
        /// </summary>
        /// <param name="message"></param>
        /// <param name="secret"></param>
        /// <returns></returns>
        public string GetSignhHash(string message, string secret)
        {
            return "5555555";
        }
    }
```

### appsetting.json
```json
"VerifySignOption": {
"Enable": true,//是否开启签名
"IsDebug": true,//是否调试，显示更多敏感信息
"Scheme": "attribute",//global与attribute两种，attribute需要向controller或者action加特式签名，global则全局
"ExpireSeconds": 60,//时间戳过期时长，单位秒
"CommonParameters": { //公共参数名的定义
	"TimestampName": "timestamp",
	"AppIdName": "appid",
	"SignName": "sign"
},
"AppSecret": {  //默认AK/SK
	"AppId": 
	}
    }
}
```
### Attribute模式使用方式
* 设置需签名的控制器或方法
```csharp
    [Route("api/v1/[controller]")]
    [VerifySign]//此控制器将签名访问
    public class WeatherForecastController : ControllerBase

    ...


    [HttpPost]
    [Route("pay/create")]
    [ProducesResponseType(200)]
    [VerifySign]//此action将签名访问
    public IActionResult Get()
```

###  忽略签名
```csharp
    [HttpPost]
    [Route("pay/create")]
    [ProducesResponseType(200)]
    [IgnoreSign]//此方法忽略签名
    public IActionResult Get()
```

生成签名
```csharp
        /// <summary>
        /// 生成签名(签名公共参数必须以url方式提供,便于查看与快速调试) 
        /// </summary>
        /// <returns></returns>
        [HttpGet("createsign")]
        public IActionResult CreateSign()
        {
            Dictionary<string, string> queryDic = new Dictionary<string, string>();
            queryDic.Add("appid", "111");//必填参数，参数名与appsetting.json的CommonParameters：AppIdName要一致
            queryDic.Add("a", "1");
            queryDic.Add("b", "1");
            var sign = SignCommon.CreateSign("secret", queryDic: queryDic, body: new { a = 1, b = "1" });
            queryDic.Add("sign", sign);
            return Ok(sign);
        }
```
