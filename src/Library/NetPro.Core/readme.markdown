
## Core使用
 [![NuGet](https://img.shields.io/nuget/v/NetPro.Core.svg)](https://nuget.org/packages/NetPro.Core)

框架的基础支撑，主要包含依赖注入，startup生命周期管理等核心逻辑

### 使用

#### appsetting.json 

```json
基础配置

"NetProOption": {
      "ProjectPrefix": "NetPro",//项目前缀
      "ProjectSuffix": "",
      "UseResponseCompression": false,
      "ThreadMinCount": 5,
      "ApplicationName": "",
      "RequestWarningThreshold": 5
	},
```
#### 依赖
此类库无依赖其他项目
