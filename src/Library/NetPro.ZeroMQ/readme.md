
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
         private readonly PublisherSocket _publisherSocket;
        public TaosService( PublisherSocket publisherSocket)
        {
              _publisherSocket = publisherSocket;
        }

        /// <summary>
        /// 发布
        /// </summary>
        public void Publish(string sql)
        { 

            _publisherSocket.SendMoreFrame("A:b") // Topic支持特殊符号，topic命名最佳实践：模块名/功能命/功能层级
                   .SendFrame(DateTimeOffset.Now.ToString());

        }
    }
```

#### 更新中...

