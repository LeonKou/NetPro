<p align="center"> <a href="https://leonkou.github.io/NetPro/"></a>
  <img  src="docs/images/netpro2.png" >
</p>

# NetPro　　　　　　　　　　　　　　　　　　　[Chinese](README_Chinese.md)
![.NET Core](https://github.com/LeonKou/NetPro/workflows/.NET%20Core/badge.svg)  ![NuGet](https://img.shields.io/nuget/v/NetPro.Startup.svg) ![NuGet](https://img.shields.io/nuget/v/NetPro.Startup.svg)

NetpPro is based on enhanced Startup features development of efficient management Startup implementation, he is cross-platform, written in C# language, and is designed to code low intrusion, low dependency, modular, pluggable, on demand reference, support dotnet3.1, dotnet5.0, dotnet6.0, due to low intrusion, References on demand can be easily upgraded for future higher versions.
# Architecture preview

<p align="center">
  <img  src="docs/images/netproinfo.png">
</p>

# Feature

Compared to the various development frameworks commonly used, NetPro has many advantages:
- It improves the development and maintenance cost caused by Startup clutter and high integration of components in the framework

- Low intrusion, based on the `NetPro.Startup` developed kit can be put into the specified path to achieve plug-in ability, easily expand new functions

- Low dependency: all development packages based on NetPro.Startup do not depend on each other

- Easily realize the development according to the business domain, through `NetPro.Startup` can easily pack the business needs into an assembly, achieve high cohesion within the business, avoid the past controller and business layer, database layer, Startup and other scattered in multiple assemblies

- Rapid development of microservices, based on NetPro.Startup best practices after the development of business modules can be quickly converted to microservices without changing the code conditions

- On-demand reference. By enhancing the Startup feature, all the toolkits developed based on NetPro.Startup can be enabled by reference

# Getting Started
All preset development kits based on NetPro.Startup please view under `src/Library`
For various development examples and best practices please view under `src/sample`
### Package Manager: 
You can install netpro.startup in your project by running the following command
```
PM> Install-Package NetPro.Startup -Version *
```

### .NET CLI : 
```
dotnet add package NetPro.Startup --version *
```

### PackageReference:
```
<PackageReference Include="NetPro.Startup" Version="*" />
```

### .NET CLI : 
```
paket add NetPro.Startup --version *
```

---

## Creating projects through scaffolding (recommended)

### 1、install netproapi[![NuGet](https://img.shields.io/nuget/v/netproapi.svg)](https://nuget.org/packages/netproapi)scaffold

Run the following command to install the scaffold
```
dotnet new -i netproapi::* 
```

### 2、Use scaffolding to create projects

Execute the following command in the specified project folder
```
dotnet new netproapi -n project name
```
For example, the current project is IAM
```
dotnet new netproapi -n IAM
```
Executing the above command will automatically create the WebApi code solution


### Execute

After the development environment runs, the effect is as follows:

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


