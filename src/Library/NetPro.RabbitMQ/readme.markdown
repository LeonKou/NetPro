
## NetPro.RabbitMQ使用


### 使用
[![NuGet](https://img.shields.io/nuget/v/NetPro.RabbitMQ.svg)](https://nuget.org/packages/NetPro.RabbitMQ)

#### 初始化Rabbit服务（代码方式）
```csharp
 services.AddRabbitMqClient(new RabbitMqClientOptions
 {
     HostName = "172.16.127.229",
     Port = 5672,
     Password = "guest",
     UserName = "guest",
     VirtualHost= "/",
 })
 .AddProductionExchange("LeonTest", new RabbitMqExchangeOptions
     {
         DeadLetterExchange = "DeadExchange",
         AutoDelete = false,
         Type = "fanout",
         Durable = true,
         Queues = new List<RabbitMqQueueOptions> { new RabbitMqQueueOptions { AutoDelete = false, Exclusive = false, Durable = true, Name = "myqueue", RoutingKeys = new HashSet<string> { "mini", "yang" } } }
     });
```

```json
"RabbitMq": {
	"HostName": "127.0.0.1",
	"Port": "5672",
	"UserName": "guest",
	"Password": "guest"
    },
"RabbitMqExchange": {
	"Type": "direct",
	"Durable": true,
	"AutoDelete": false,
	"DeadLetterExchange": "default.dlx.exchange",
	"RequeueFailedMessages": true,
	"Queues": [
		{
		 "Name": "myqueue",
		 "RoutingKeys": [ "routing.key" ]
		}]
	}
```
### 生产消息

```csharp

namespace MQ.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly IQueueService _queueService;
        public ValuesController(IQueueService queueService)
        {
            _queueService = queueService;
        }
        // GET api/values
        [HttpGet]
        [Route("Send")]
        public ActionResult<IEnumerable<string>> Send(string mes)
        {
            _queueService.Send(//发送消息
                                @object: $"{mes}",
                                exchangeName: "LeonTest",
                                routingKey: "yang"
                                 );
            return new string[] { "value1", "value2" };
        }
    }
}
 }
```
### 消费消息
```csharp
namespace ReceiveB.Service
{
    //继承IMessageHandler，即可作为消费者消费消息
    public class CustomMessageHandler : IMessageHandler
    {
        public CustomMessageHandler()
        {
        }

        public void Handle(string message, string routingKey)
        {
            // 消费消息
            Console.WriteLine($"这是B--------------信息为{message}routekey为{ routingKey}");
            _logger.LogInformation("Ho-ho-hoooo");
        }
    }
}
```
