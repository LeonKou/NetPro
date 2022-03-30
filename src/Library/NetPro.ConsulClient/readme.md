
## NetPro.Proxy使用
 [![NuGet](https://img.shields.io/nuget/v/NetPro.ConsulClient.svg)](https://nuget.org/packages/NetPro.ConsulClient)

Consul 客户端

### 使用
- 健康检查默认以TTL模式，10秒检查

- 如在容器中使用，会自动发现容器ip与程序分配的默认端口。

- 如需要指定ip地址需配置如下LANIP和PORT环境变量，默认遍历本机所有网卡地址的第一个与分配给程序的默认端口，示例如下：
宿主机IP地址   LANIP=192.168.1.1
端口号 PORT=5000

#### appsetting.json 

- 增加以下配置节点
```json
  "ConsulOption": {
    "Enabled": false, //是否开启,不配置默认开启
    "ServiceName": "xxx.api", //可留空；留空默认以入口程序名作为servicename
    "EndPoint": "http://localhost:8500",
    "WaitTime": null,
    "Tags": [ "HUHU" ], //可留空
    "Datacenter": null, //可留空
    "Token": null //可留空
  }
```
如果基于NetPro.WebApi的项目，只需要以上配置即可，如不是基于NetPro.WebApi的项目，需手动按一下初始化组件
#### 启用服务
``` csharp
public void ConfigureServices(IServiceCollection services, IConfiguration configuration = null)
        {
            services.AddConsul(configuration);
        }

public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseConsuClient();
        }
```

```csharp
ublic class ConsulDemoController : ControllerBase
    {

        private readonly ILogger<TimeZoneDemoController> _logger;
        private readonly IConsulClient _consulClient;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="consulClient"></param>
        public TimeZoneDemoController(ILogger<TimeZoneDemoController> logger,
        IConsulClient consulClient)
        {
            _logger = logger;
            _consulClient=consulClient;
        }

        /// <summary>
        /// consul发现服务
        /// </summary>
        [HttpGet("DiscoveryServices")]
        [ProducesResponseType(200, Type = typeof(ResponseResult))]
        public async Task<IActionResult> DiscoveryServices(string serviceName = "XXX.API")
        {
            //以下几种方式都可拿到注册的服务地址
            var result = await _consulClient.Agent.DiscoveryAsync();
            var result1 = await _consulClient.Catalog.DiscoveryAsync(serviceName);
            var result2 = await _consulClient.DiscoveryAsync(serviceName);
            return Ok(new { result, result1, result2 });
        }
```
