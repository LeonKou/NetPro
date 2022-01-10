
# NetPro.MQTTClient 使用
 [![NuGet](https://img.shields.io/nuget/v/NetPro.MQTTClient.svg)](https://nuget.org/packages/NetPro.MQTTClient)

MQTT客户端类库，支持多服务端，完整示例
## 使用
安装引用NetPro.MQTTClient

先增加如下配置

```json
"MQTTClientOption": {
		"Enabled": true,
		"ConnectionString": [
			{
				"Key": "1", //连接串key别名，唯一
				"Value": "clientid=netpro;host=mqtt://127.0.0.1:1883;username=netpro;password=netpro;timeout=5000;keepalive=120;cleansession=true;" //别名key对应的连接串
			}
		]
	}
```
### 1、基于NetPro.Startup基座

增加环境变量
ASPNETCORE_HOSTINGSTARTUPASSEMBLIES=NetPro.Satrtup 

### 2、无依赖使用

```csharp
public void ConfigureServices(IServiceCollection services)
{ 
   services.AddMQTTClient(configuration);      
}
```

##### 服务注入使用
```csharp 

 public class HttpProxyController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly  MqttClientMulti _mqttClientMulti;
        //构造函数注入
        public HttpProxyController(
            ILogger<DatabaseCurdController> logger
            MqttClientMulti _mqttClientMulti)
        {
            _logger = logger;
            _exampleProxy = exampleProxy;
        }

        [HttpGet("getorcreate")]
        [PostResponseCache(Duration = 2)]
        [ProducesResponseType(200, Type = typeof(ResponseResult))]
        public async Task<IActionResult> GetOrCreateAsync(uint id)
        {
            var _mqttClient = _mqttClientMulti["1"];
            var messagePayload = new MqttApplicationMessageBuilder()
                                .WithTopic("netpro/local")//共享标识$queue和$share
                                .WithPayload($"发布消息-{DateTime.Now}")
                                .WithAtMostOnceQoS()//WithAtMostOnceQoS:Level0ithAtLeastOnceQoS:Level1;WithExactlyOnceQoS:Level2
                                .WithRetainFlag(true)//服务器保持消息，有客户端连接此主题后一条消息；一个主题保留一条消息；
                                 .Build();           //删除消息既发送一条Payload为0的消息即可。

            _mqttPublishClient.PublishAsync(messagePayload);
            return Ok();
        }
    }
```
