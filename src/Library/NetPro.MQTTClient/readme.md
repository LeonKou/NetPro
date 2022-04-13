
# NetPro.MQTTClient 使用
 [![NuGet](https://img.shields.io/nuget/v/NetPro.MQTTClient.svg)](https://nuget.org/packages/NetPro.MQTTClient)

MQTT客户端类库，支持多服务端，完整示例
## 使用
安装引用NetPro.MQTTClient

先增加如下配置

> clientid不指定自动生成，如手动指定必须保证在broker中唯一！

```json
"MQTTClientOption": {
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
> 当想自定义连接字符串获取方式时，无论是否基于NetPro.Web.Api, 都能通过传入委托来自定义连接字符串获取方式：

```c#
public void ConfigureServices(IServiceCollection services)
{
    services.AddMQTTClient(GetConnectionString);
}

 public IList<ConnectionString> GetConnectionString(IServiceProvider serviceProvider)
 {
     var connector = new List<ConnectionString>();
     connector.Add(new ConnectionString { Key = "2", Value = "clientid=netpro;host=mqtt://192.168.100.187:1883;username=netpro;password=netpro;timeout=5000;keepalive=2;cleansession=true;" });
     return connector;
 }
```
##### 服务注入使用
发布者
```csharp 

 public class HttpProxyController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly  IMqttClientMulti _mqttClientMulti;
        //构造函数注入
        public HttpProxyController(
            ILogger<DatabaseCurdController> logger
            IMqttClientMulti _mqttClientMulti)
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
                                .WithTopic("netpro/local")
                                .WithPayload($"发布消息-{DateTime.Now}")
                                .WithAtMostOnceQoS()//WithAtMostOnceQoS:Level0ithAtLeastOnceQoS:Level1;WithExactlyOnceQoS:Level2
                                .WithRetainFlag(true)//服务器保持消息，有客户端连接此主题后一条消息；一个主题保留一条消息；
                                 .Build();           //删除消息既发送一条Payload为0的消息即可。

            _mqttPublishClient.PublishAsync(messagePayload);
            return Ok();
        }
    }
```
消费者
- 注意：MQTT5.0支持共享订阅实现订阅者负载，利用共享标识$queue和$share即可
```C#
 public class MQTTClientTask : IStartupTask
    {
        private readonly IMqttClientMulti _mqttClientMulti;
        public MQTTClientTask()
        {
            _mqttClientMulti = EngineContext.Current.Resolve<IMqttClientMulti>();
        }

        public int Order => 2;

        public void Execute()
        {
            var filter = new MqttTopicFilter()
            {
                //https://www.hivemq.com/blog/mqtt-client-load-balancing-with-shared-subscriptions/
                //共享标识$queue和$share;
                Topic = "$share/g/netpro",//"家/客厅/空调/#",#:通配符;topic通过/分割主题层级，一般层级由高到低 ;共享标识$queue和$share
                QualityOfServiceLevel = MqttQualityOfServiceLevel.AtMostOnce,
            };

            _Subscribe();
            void _Subscribe()
            {
                var _mqttClient = _mqttClientMulti["1"];
                var result = _mqttClient.SubscribeAsync(filter).ConfigureAwait(false).GetAwaiter().GetResult();
                //消费消息
                _mqttClient.ApplicationMessageReceivedHandler = new MqttApplicationMessageReceivedHandlerDelegate(arg =>
                {
                    string payload = System.Text.Encoding.UTF8.GetString(arg.ApplicationMessage.Payload);
                    System.Console.WriteLine("Message received, topic [" + arg.ApplicationMessage.Topic + "], payload [" + payload + "]");
                });
                //重连
                _mqttClient.DisconnectedHandler = new MqttClientDisconnectedHandlerDelegate(async arg =>
                {
                    //只是重连，但是消息需要重新订阅;也可设置CleanSession为false，重连依旧启用之前的订阅。
                    var reconnectResult = await _mqttClient.ReconnectAsync();
                    _Subscribe(); //CleanSession设置为false后，可不必重复订阅。
                });
            }
        }
    }
```
