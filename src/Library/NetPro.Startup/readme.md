
# NetPro.Satrtup使用

 [![NuGet](https://img.shields.io/nuget/v/NetPro.Satrtup.svg)](https://nuget.org/packages/NetPro.Satrtup)
无代码侵入性的Startup初始化插件

## 使用

### 启用服务

通过添加环境变量ASPNETCORE_HOSTINGSTARTUPASSEMBLIES=NetPro.Satrtup，开发环境可以通过设置launchSettings.json来启用NetPro.Satrtup插件

```json
{ 
  "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development",
        "ASPNETCORE_HOSTINGSTARTUPASSEMBLIES": "NetPro.Satrtup" 
      }
}

```

生产环境通过设置系统环境方式启用以docker为例

```shell
docker run -e ASPNETCORE_HOSTINGSTARTUPASSEMBLIES=NetPro.Satrtup
```

k8s为例

```yaml
env:
- name:ASPNETCORE_HOSTINGSTARTUPASSEMBLIES
  value:"NetPro.Satrtup"

```
linux为例

```shell
export ASPNETCORE_HOSTINGSTARTUPASSEMBLIES=NetPro.Satrtup
```

## 高级用法

### 更改Startup执行顺序，既中间件的执行顺序

增加startup.json 文件到程序运行目录

```json
{
	"NetProCoreStartup": 0,
	"NetProCoreStartup1": 1
}

//key为继承INetProStartup的实现类名称
//value为对应的执行顺序
//不区分大小写
//数字越大越靠后执行

```

