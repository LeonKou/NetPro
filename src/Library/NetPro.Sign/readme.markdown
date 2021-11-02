
## 接口签名


主要防范请求参数被篡改和增加爬虫难度，签名组件应该在所有中间件之前执行，以保证其他组件不影响签名的正常执行(签名组件如在拦截类型的缓存中间件等之后执行，会让大部分请求绕过签名直接请求成功)

### 接口签名使用
默认为url参数与body参数根据参数名升序排序合并成一个字符串再utf-8编码后进行摘要计算，得到的值转为16进制小写
例如http://localhost:5000/api/user?timestamp=111111&appid=knasdfnas&name=yuhun&age=17&sign=jasdfksnlfsmf98sdflmdf8
body:{"police":"noPo"}

签名规则：将query参数名和"body"升序排序后：
HMACSHA256(body={"police":"noPo"}&appid=knasdfnas&age=17&name=yuhun&timestamp=111111,secret)

如果是md5,则在query参数末尾追加secret
md5(body={"police":"noPo"}&appid=knasdfnas&age=17&name=yuhun&timestamp=111111+secret)

startup注入

``` csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddVerifySign(s =>
            {
                s.OperationFilter<VerifySignCustomer>();//VerifySignCustomer为自定义摘要与获取secret，如默认规则。则不需要OperationFilter
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
        /// <param name="appid"></param>
        /// <returns></returns>
        public string GetSignSecret(string appid)
        {
            var secret = "1111";//自定义通过appid获取对应的secret
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
            return "5555555";//对message进行摘要，secret作为干扰项
        }
    }
```

### appsetting.json
```json
"VerifySignOption": {
"Enabled": true,//是否启用
"IsForce":true,//是否强制实名校验  ，false 签名错误只记录日志
"IsDebug": true,//是否调试，显示更多敏感信息action加特式签名，global则全局
"ExpireSeconds": 60,//时间戳过期时长，单位秒
"CommonParameters": { //公共参数名的定义
	"TimestampName": "timestamp",
	"AppIdName": "appid",
	"SignName": "sign"
},
"AppSecret": {  //默认AK/SK
	"AppId":{
	    "你的appid1": "对应的secret1",
	    "你的appid2": "对应的secret2"
	} 
    }
}
```
### Attribute模式使用方式(废弃，签名只适合中间件方式)
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

###  忽略签名(废弃，此特性在中间件中无效)
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
            object body=new { a = 1, b = "1" };
            var query = HttpUtility.ParseQueryString(string.Empty);
            query["appid"] = "111";       //必传 应用id    
            query["acount"] = "我是你+"; //必传；加密方法

            long timestamp=SignCommon.CreateTimestamp();
             query["timestamp"] = timestamp;    //必传；时间戳                     
            var sign = SignCommon.CreateSign("secret", queryDic: query, body: body);//如果为Get请求，Body参数为空即可
             query["sign"] =sign;    //必传；加密方法
            //得到的queryDic便是完整url参数字典
            return Ok(sign);
        }
```
