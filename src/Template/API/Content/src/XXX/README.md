# 说明

## 最佳实践（主观）

- 当实践微服务架构时，建议单个微服务不做过多业务，保持每个服务足够小，足够稳健，不做过多分层，最好一个服务就只有一个api或者grpc入口程序集，不要划分统一的Entity,Service,Respository等做单一功能的程序集。如果要利用多个程序集来保证清晰度和维护成本表示已突破微服务最佳实践，导致已掉入单个服务过大，复杂，冗余的陷阱。

- 当实践单一项目时，非微服务实践，推荐在单个项目中实现所有功能以加快开发和调试，维护成本，此场景由于业务不断增加，复杂度提高，建议单个业务模块一个程序集来清晰划分业务，以达到业务间过少耦合，调用，为之后服务升级，拆分做好预备工作的同时也能不增加当前开发成本。

- 如前期没想好如何划分服务，将以每个模块按程序集隔离，内部聚合Controller、Service、DO、DTO、Startup

- 配置建议分块存储，不要混合到一个json文件中，建议通过appsetting.json配置ConfigPath节点将配置集中存放到其他目录，存放到此节点下的json配置会自动加载。

- 当在构建大型系统时，建议使用配置中心,由于配置一般是变更较少，常见配置中心客户端都支持持久化到本地，故配置中心是否高可用可能并不是影响稳定性的关键因素，Apollo繁重，推荐使用Net社区的AgileConfig配置中心[agile-config](https://github.com/dotnetcore/AgileConfig)

- 建议中间件按需引用，不要引用过多用不到中间件，以防对问题排查带来过多干扰，每个中间件的初始化尽量有对应的Startup类文件放于Startup文件夹中。

## 文件夹划分规则：

除了Startup和Protos文件夹外，其他都是以一个业务一个文件夹划分

每个`大业务模块`一个文件夹

在业务模块下每个`功能层`一个文件夹

```
├── Examples(大业务模块)
│       ├── Controller （控制器层） 功能层
│       ├── Model（输入输出model实体）功能层
│       ├── Service（业务服务层）功能层
│       ├── Mapper （实体映射层）功能层
│       └── Proxy（远程请求层）功能层
│        ...
│
├── GlobalizationDemo(大业务模块)
│       ├── Controller （控制器层） 功能层
│       ├── Model（输入输出model实体）功能层
│       ├── Service（业务服务层）功能层
│       ├── Mapper （实体映射层）功能层
│       └── Proxy（远程请求层）功能层
│        ...
├──TimeZoneDemo(大业务模块)
│       ├── ...
│ ...

```

## 其他说明

### Startup 文件夹
此处存放继承了`INetProStartup`接口的依赖注入的startuo实现类，执行顺序依靠实现类的Order属性控制，可在控制台和自动生成的startup.json文件中查看执行顺序并支持自定义，Order值越大越靠后执行

### 配置中心AgileConfig

#### 安装
```xml
<PackageReference Include="AgileConfig.Client" Version="1.6.2.1" />
```

#### 初始化 
Program.cs
```C#

var host = Host.CreateDefaultBuilder(args)
               .ConfigureAppConfiguration((context, config) =>
               {
                   config.AddAgileConfig();
               })
               .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup("NetPro.Startup"));
host.Build().Run();
```

#### 配置
appsetting.json
```json
"AgileConfig": {
    "appId": "Common",
    "secret": "Common",
    "nodes": "http://agile-config.com/",//多个node逗号分割
    "name": "客户端名称",
    "tag": "逗号分割tag",
    "env": "DEV"//DEV、TEST、STAGING、PROD
  }
```

#### 注意事项
- 以本地配置为最准
- 公共应用作为公开配置可被继承
- 关联公共应用可集成公共配置，并且同节点支持覆写
例如：

公共配置
```json
{
  "Commonconf": {
    "CustomDllPattern": "1111111",
    "MountePath": "111111"
  }
}
```
私有配置
```json
{
  "Commonconf": {
    "CustomDllPattern": "22222",
  }
}
```

最准私有配置生效的既是
```json
{
  "Commonconf": {
    "CustomDllPattern": "22222",
     "MountePath": "111111"
  }
}
```

