<p align="center">
  <img  src="docs/images/netpro2.png" >
</p>

# NetPro　　　　　　　　　　　　　　　　　　　[English](README.md)
![.NET Core](https://github.com/LeonKou/NetPro/workflows/.NET%20Core/badge.svg)  ![NuGet](https://img.shields.io/nuget/v/NetPro.Startup.svg) ![NuGet](https://img.shields.io/nuget/v/NetPro.Startup.svg)

.NetpPro是基于增强启动特性开发的高效管理Startup的实现，他是跨平台的，用C#语言编写，并被设计成代码侵入性低、依赖低、模块化、可插拔、按需引用，支持dotnet3.1、dotnet5.0、dotnet6.0 ,由于侵入性低，按需引用，可轻松升级未来的更高版本。

# 架构预览 

<p align="center">
  <img  src="docs/images/netproinfo.png">
</p>

# 特点

与平常使用的各种开发框架相比，NetPro具有许多有点：
- 改进了以往Startup杂乱，框架对各组件的集成度过高导致的开发维护成本

- 侵入性低，基于`NetPro.Startup`开发的工具包可放入指定路径即可实现插件化能力，轻松扩展新功能

- 依赖低，所有基于NetPro.Startup的开发包互相都无依赖

- 轻松实现按业务领域开发，通过`NetPro.Startup`可轻松将业务所需打包进一个程序集中，实现业务内的高内聚，避免以往控制器与业务层、数据库层、Startup等散落在多个程序集。

- 微服务快速开发，基于NetPro.Startup最佳实践开发业务模块后在不改代码条件下即可快速转为微服务。

- 按需引用，通过增强启动特性，所有基于NetPro.Startup开发的工具包都可实现引用即启用，

# Getting Started
所有基于NetPro.Startup的预置开发包请在 `src/Library`下查看


### Package Manager方式: 
你可以运行以下下命令在你的项目中安装NetPro.Startup
```
PM> Install-Package NetPro.Startup -Version *
```

### .NET CLI 方式: 
```
dotnet add package NetPro.Startup --version *
```

### PackageReference:
```
<PackageReference Include="NetPro.Startup" Version="*" />
```

### .NET CLI 方式: 
```
paket add NetPro.Startup --version *
```

---

## 通过脚手架创建项目(推荐)

### 1、安装netproapi[![NuGet](https://img.shields.io/nuget/v/netproapi.svg)](https://nuget.org/packages/netproapi)脚手架
执行以下命令安装脚手架
```
dotnet new -i netproapi::* 
```

### 2、使用脚手架创建项目

在指定的项目文件夹中执行以下命令
```
dotnet new netproapi -n 项目名称
```
例如当前项目为IAM
```
dotnet new netproapi -n IAM
```
执行以上命令将自动创建WebApi代码解决方案


### 运行

开发环境运行后效果如下：

```
 ____  _____        _   _______
|_   \|_   _|      / |_|_   __ \
  |   \ | |  .---.`| |-' | |__) |_ .--.   .--.
  | |\ \| | / /__\\| |   |  ___/[ `/'`\]/ .'`\ \
 _| |_\   |_| \__.,| |, _| |_    | |    | \__. |
|_____|\____|'.__.'\__/|_____|  [___]    '.__.'


[17:40:03] dotnet process id:14520
The enhanced service has started
[17:40:04] loading json files
Service injection sequence：
[17:40:15] apollo已关闭
info: NetProSwaggerServiceExtensions[0]
      NetPro Swagger 已启用
--------------------------------------------------------------------------------------------------------------------------------------
|    Order   |       StartupClassName       |                   Path                   |             Assembly            |  Version  |
--------------------------------------------------------------------------------------------------------------------------------------
| 0          | NetProCoreStartup            | NetPro.Core.Startup.NetProCoreStartup    | NetPro.Core                     |  3.1.13.0 |
--------------------------------------------------------------------------------------------------------------------------------------
| 0          | CheckerStartup               | NetPro.Analysic.CheckerStartup           | NetPro.Checker                  |  3.1.13.0 |
--------------------------------------------------------------------------------------------------------------------------------------
| 0          | ErrorHandlerStartup(default) | NetPro.Web.Api.ErrorHandlerStartup       | NetPro.Web.Api                  |  3.1.13.0 |
--------------------------------------------------------------------------------------------------------------------------------------
| 100        | SwaggerStartup               | NetPro.Swagger.SwaggerStartup            | NetPro.Swagger                  |  3.1.13.0 |
--------------------------------------------------------------------------------------------------------------------------------------
| 100        | StaticFilesStartup(default)  | NetPro.Web.Api.StaticFilesStartup        | NetPro.Web.Api                  |  3.1.13.0 |
--------------------------------------------------------------------------------------------------------------------------------------
| 200        | RoutingStartup(default)      | NetPro.Web.Api.RoutingStartup            | NetPro.Web.Api                  |  3.1.13.0 |
--------------------------------------------------------------------------------------------------------------------------------------
| 300        | CorsStartup                  | NetPro.Web.Api.CorsStartup               | NetPro.Web.Api                  |  3.1.13.0 |
--------------------------------------------------------------------------------------------------------------------------------------
| 400        | ShareBodyStartup             | NetPro.ShareRequestBody.ShareBodyStartup | NetPro.NetProShareRequestBody   |  3.1.13.0 |
--------------------------------------------------------------------------------------------------------------------------------------
| 1000       | EndpointsStartup(default)    | NetPro.Web.Api.EndpointsStartup          | XXX.API (custom)                |  1.0.0.0  |
--------------------------------------------------------------------------------------------------------------------------------------
| 1000       | NetProCsRedisStartup         | NetPro.CsRedis.NetProCsRedisStartup      | NetPro.CsRedis                  |  3.1.13.0 |
--------------------------------------------------------------------------------------------------------------------------------------
| 2147483647 | ApiStartup                   | XXX.API.ApiStartup                       | XXX.API (custom)                |  1.0.0.0  |
--------------------------------------------------------------------------------------------------------------------------------------
| 2147483647 | ApiStartup-1                 | XXX.Plugin.Web.Demo.ApiStartup           | XXX.Plugin.Web.Demo (custom)    |  1.0.0.0  |
--------------------------------------------------------------------------------------------------------------------------------------
| 2147483647 | ApiStartup-2                 | XXX.Plugin.Web.Manager.ApiStartup        | XXX.Plugin.Web.Manager (custom) |  1.0.0.0  |

```


