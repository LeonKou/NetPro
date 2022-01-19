
## EasyNetQ使用
 [![NuGet](https://img.shields.io/nuget/v/NetPro.EasyNetQ.svg)](https://nuget.org/packages/NetPro.EasyNetQ)

对EasyNetQ的封装，简化使用，支持多库

### 使用

#### appsetting.json 

```json
"EasyNetQOption": {
    "ConnectionString": [
      {
        "Key": "rabbit2", //连接串key别名，唯一
        "Value": "host=192.168.18.129:5672;virtualHost=/;username=admin;password=123456;timeout=60" //别名key对应的连接串
      },
      {
        "Key": "rabbit1", //连接串key别名，唯一
        "Value": "host=192.168.18.129:5672;virtualHost=/;username=admin;password=123456;timeout=60" //别名key对应的连接串
      }
    ]
  },

```
#### 启用服务
没有基于NetPro.Web.Api 的使用场景，必须手动进行初始化，如下：
```csharp
IConfiguration Configuration;

public void ConfigureServices(IServiceCollection services)
{    
       services.AddEasyNetQ(Configuration);
}
```

基于NetPro.Web.Api的使用，只需要添加引用后配置以上appsetting.josn配置EasyNetQOption节点即可

### 使用说明

```csharp
 public class RabbitmqService : IRabbitmqService
    {
        private readonly EasyNetQMulti _easyNetQMulti
        EasyNetQMulti easyNetQMulti)
        {
            _easyNetQMulti=easyNetQMulti;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Method()
        {
             //订阅/Subscribe
                    _idbus ["rabbit2"].Subscribe<TextMessage>("subscriptionId", tm =>
                    {
                        Console.WriteLine("Recieve Message: {0}", tm.Text);
                    });

           //发布/Publish
                using (var bus = _easyNetQMulti["rabbit21"])
                {
                    var input = "";
                    Console.WriteLine("Please enter a message. 'q'/'Q' to quit.");
                    while ((input = Console.ReadLine()).ToLower() != "q")
                    {
                        bus.PubSub.Publish(new TextMessage
                        {
                            Text = input
                        });
                    }
                }
        }
    }
```

#### 更新中...
reference
 [使用rabbitmq消息队列——EasyNetQ插件介绍](https://www.cnblogs.com/shanfeng1000/p/12359190.html)

