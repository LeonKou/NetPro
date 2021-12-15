
## NetPro.Cors使用
 [![NuGet](https://img.shields.io/nuget/v/NetPro.Cors.svg)](https://nuget.org/packages/NetPro.Cors)

跨域组件

### 使用

- 只支持基于NetPro.Core基础上使用，跨域策略名称为 CorsPolicy
#### appsetting.json 

- 增加以下配置节点即生效
```json

	"NetProCorsOption": {
		"CorsOrigins": "*" //跨域配置,不填未开启跨域,没有此节点默认允许所有跨域
	}


```
