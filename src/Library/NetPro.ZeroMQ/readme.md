
## NetPro.ZeroMQ使用
 [![NuGet](https://img.shields.io/nuget/v/NetPro.ZeroMQ.svg)](https://nuget.org/packages/NetPro.ZeroMQ)

### 使用

#### appsetting.json 

```json
  "ZeroMQOption": {
    "PublishPort": 81,
    "PushPort": 82
  }

```
#### 启用服务
没有基于NetPro.Web.Api 的使用场景，必须手动进行初始化，如下：
```csharp
IConfiguration Configuration;

public void ConfigureServices(IServiceCollection services)
{
            var option= new ZeroMQOption(configuration);
            services.AddZeroMQForPublisher(option);
            services.AddZeroMQForPushSocket(option);
}
```

基于NetPro.Web.Api的使用，只需要添加引用后配置以上appsetting.josn配置ZeroMQOption节点即可

### 使用说明
```csharp
 public class ZeroMQService: IZeroMQService
    {
        private readonly static object _lock = new();//zeromq socket is thread-unsafe
        private readonly PublisherSocket _publisherSocket;
        private readonly PushSocket _pushSocket;
        public ZeroMQService( PublisherSocket publisherSocket
                , PushSocket pushSocket)
        {
            _publisherSocket = publisherSocket;
            _pushSocket = pushSocket;
        }

        /// <summary>
        /// publish-subscribtion
        /// </summary>
        public void Publish(string sql)
        { 
            lock (_lock)
            {
            _publisherSocket.SendMoreFrame("A:b") // Topic支持特殊符号，topic命名最佳实践：模块名/功能命/功能层级
                   .SendFrame(DateTimeOffset.Now.ToString());
            }
        }

        /// <summary>
        /// push-pull
        /// </summary>
        /// <returns></returns>
        [HttpGet("PushSocket")]
        [ProducesResponseType(200, Type = typeof(ResponseResult))]
        public IActionResult PushSocket()
        {
            lock (_lock)
            {
                //推数据 https://github.com/zeromq/netmq/blob/ea0a5a7e1b77a1ade9311f187f4ff37a20d5d964/src/NetMQ.Tests/PushPullTests.cs
                _pushSocket.SendFrame("Hello Clients"); ;
            }
            return Ok();
        }
    }
```

#### 更新中...

