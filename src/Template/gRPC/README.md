# 代码脚手架

## 创建代码模板

- 1、创建根目录Content(名字不定，)
- 2、在根目录下放置完整代码目录结构
- 3、在代码目录下增加.template.config文件夹(前面带".")
- 4、在.template.config文件夹目录下增加template.json描述文件

```json
{
  "$schema": "http://json.schemastore.org/template",
  "author": "Leon",
  "classifications": [ "gRPC" ],
  "name": "NetPro gRPC",
  "identity": "NetPro gRPC", //模板唯一标识
  "groupIdentity": "NetPro gRPC",
  "shortName": "netprogrpc", //【修改】短名称，使用 dotnet new <shortName> 安装模板时的名称
  "tags": {
    "language": "C#",
    "type": "project"
  },
  "sourceName": "XXX", //【修改】在使用 -n 选项时，会替换模板中项目的名字
  "preferNameDirectory": true
}
```
## 安装模板脚手架

### 1、本地安装

本地安装脚手架即本地将模板代码以dotnet tool方式安装
在模板代码根目录,这里是Content文件夹同级目录,执行命令
```
dotnet new -i ./ 
```
-i 后面路径即时模板代码的根目录，此处是Content目录

### 2、nuget方式安装

- 1、下载nuget.exe
- 2、在Content统计目录下新增扩展名为`.nuspec`的nuget描述文件

```xml
<?xml version="1.0" encoding="utf-8"?>
<package xmlns="http://schemas.microsoft.com/packaging/2012/06/nuspec.xsd">
  <metadata>
    <id>netproapi</id><!-- 唯一标识 -->
    <version>1.0.0</version><!-- 版本 -->
    <description>
      NetProGrpc Template, including Grpc     
    </description>
    <authors>leon</authors>
    <packageTypes>
      <packageType name="Template" /><!-- 固定Template -->
    </packageTypes>
  </metadata>
</package>
```

- 3、打包nuget。

如果安装了NuGet package Explorer.exe双击即可进入gui界面，点击Save即可生成nupkg的nuget包。
如果安装了nuget.exe ，执行命令进行打包生成xxx.nupkg：
```
nuget pack  xxx/xxx.nuspec -NoDefaultExcludes -OutputDirectory . 
```
- 4、上传nuget到服务器
- 5、以nuget方式安装脚手架,执行以下命令安装
```
 dotnet new -i netproapi::*   # -i：安装 
```

- 6、使用模板脚手架

```
dotnet new netproapi -n 项目名称。
```

7、 列出所有自定义模板的卸载命令

```
 dotnet new -u 
```