
#### 接口签名使用
默认为url参数与body参数合并成一个字符串再utf-8编码后进行摘要计算，得到的值转为16进制小写

startup注入

``` csharp
services.AddVerifySign(s =>
            {
                s.OperationFilter<VerifySignCustomer>();
            });
```

#### 自定义摘要算法

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

#### appsetting.json
```json
"VerifySignOption": {
		"Enable": true,//是否开启签名
		"ExpireSeconds": 60,//时间戳过期时长，单位秒
		"CommonParameters": { //公共参数名的定义
			"TimestampName": "timestamp",
			"AppIdName": "appid",
			"SignName": "sign"
		},
		"AppSecret": {  //默认AK/SK
			"AppId": {

			}
		}
	}
```
