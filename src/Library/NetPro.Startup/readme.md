
# NetPro.Satrtup使用

 [![NuGet](https://img.shields.io/nuget/v/NetPro.Satrtup.svg)](https://nuget.org/packages/NetPro.Satrtup)
无代码侵入性的Startup初始化插件

## 使用

### 约定
默认站点根目录下所有json自动在配置中生效，通过IConfiguration获取配置，有一些例外的json文件不在此生效：
`runtimeconfig.template.json`；`startup.json`；`global.json`

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

### 指定配置文件路径（用于整理拆分配置文件）：

在`appsettings.json`文件中增加配置项`ConfigPath`，指定配置文件夹存放路径，例如此处，`appsetting.json`中只有这一条配置项：

```json
{
  "ConfigPath": "ConfigJsons"
}
```

所有在程序运行目录`ConfigJsons`中的json文件将被读取为配置项，此时，原有的`appsetting.json`被拆分为：

```plaintext
E:.
|   appsettings.json
|   Project.csproj
|
\---ConfigJsons
        globalization.json
        log.json
        mq.json
        redis.json
        swagger.json
```

注：这里的配置文件同样支持按环境适配，比如在开发环境下，`mq.Development.json`文件中的相应配置项将会生效

#### 可覆盖

在`appsetting.json`中可以配置`Overridable`属性，这个变量用来决定在`appsetting.json`和`ConfigPath`中配置文件有相同的配置节点时，是否用`ConfigPath`中配置文件的节点来覆盖`appsetting.json`中的配置节点，当不配置时默认为`true`，即存在相同配置节点时，`appsetting.json`中的配置节点会被覆盖，调整这个属性为`false`有助于在使用`ConfigJsons`整理配置项的同时，将一些经常变动的配置项集中到`appsetting.json`进行调整

```json
{
  "Overridable": true
}
```