
## Swagger使用
 [![NuGet](https://img.shields.io/nuget/v/NetPro.Swagger.svg)](https://nuget.org/packages/NetPro.Swagger)

对swagger的封装，包含认证，默认头，默认公共query参数，文件上传

### 使用

#### appsetting.json 

```json
"SwaggerOption": {
 "Enabled": true, //默认启用
 "RoutePrefix": "swagger",
 "MaxDisplayedTags":"100"//最多显示几个Controller,不写默认100
 "ServerPrefix": "lu/", //json endpoint，用于反向代理场景使用
 "OAuth2":{
      "Server":"www.baidu.com",//开启Oauth2功能
      "Scopes": [
				{
					"Key": "d",
					"Value": "b"
				}
			]
 }
 "Description": "this is swagger for netcore",
 "Title": "Demo swagger",
 "Version": "first version",
 "TermsOfService": "netcore.com",
 "Contact": {
 	"Email": "swagger@netcore.com",
 	"Name": "swagger",
 	"Url": "swagger@netcore.com"
 },
 "License": {
 	"Name": "",
 	"Url": ""
 },
 "Headers": [ //swagger默认头参数
 	{
 	"Name": "User",
 	"Description": "用户"
 	}
 ],
 "Query": [ //swagger默认url公共参数
 	{
 	"Name": "sign",
 	"Description": "签名"
 	},
 	{
 	"Name": "timestamp",
 	"Description": "客户端时间戳"
 	}
 ]
}

```
#### 启用服务
```csharp
public void ConfigureServices(IServiceCollection services,IConfiguration configuration)
{
     services.AddNetProSwagger(configuration);
}

public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
      app.UseNetProSwagger();
}
```

### 注意
webapi项目中不能包含Web相关组件，例如
Microsoft.VisualStudio.Web.CodeGeneration.Design
否则会导致swagger异常