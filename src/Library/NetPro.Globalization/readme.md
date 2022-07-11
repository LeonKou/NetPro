
# NetPro.Globalization使用
 [![NuGet](https://img.shields.io/nuget/v/NetPro.Globalization.svg)](https://nuget.org/packages/NetPro.Globalization)

国际化多语言使用，支持官方资源文件的方式外底层增加了sqlite持久化存储方式，便于跨语言跨项目共享和检查遗漏的多语言

此项目通过修改了[Localization.SqlLocalizer](https://github.com/damienbod/AspNetCoreLocalization)源码获得支持，特此感谢[damienbod](https://github.com/damienbod/AspNetCoreLocalization)

## 使用

### 初始化服务

#### 无依赖方式

没有依赖 [![NuGet](https://img.shields.io/nuget/v/NetPro.Core.svg)](https://nuget.org/packages/NetPro.Core)的使用，需要手动初始化注入
```csharp
  public void ConfigureServices(IServiceCollection services)
   {
       services.AddGlobalization();
   }

  public void Configure(IApplicationBuilder application)
   {
    //置于app.UseRouting()后便可;
    var configuration = app.ApplicationServices.GetService<IConfiguration>();

    var globalization = configuration.GetSection(nameof(Globalization)).Get<Globalization>();

    var cultures = globalization?.Cultures ?? new string[] { };

    var localizationOptions = new RequestLocalizationOptions()
        .AddSupportedUICultures(cultures)
        //.AddSupportedCultures(cultures)
        ;
    localizationOptions.RequestCultureProviders.Insert(0, new QueryStringRequestCultureProvider { UIQueryStringKey = globalization.UIQueryStringKey });
    localizationOptions.RequestCultureProviders.Insert(1, new AcceptLanguageHeaderRequestCultureProvider());
    localizationOptions.RequestCultureProviders.Insert(2, new CookieRequestCultureProvider());
    app.UseRequestLocalization(localizationOptions);
   }
```

appsetting.json

```json
{
	"Globalization": {
		"UIQueryStringKey": "language",//请求的query携带的多语言参数名
		"ConnectionString": "Data Source=LocalizationRecords.sqlite", //sqlite地址
		"Cultures": [
			"zh-CN",
			"en-US"
		],
		"Annotations": true, //是否打开注册数据注解本地化服务
		"Record": false //不存在是否记录(自动插入数据库默认语系)，默认true
	}
}

```

#### 依赖于[![NuGet](https://img.shields.io/nuget/v/NetPro.Core.svg)](https://nuget.org/packages/NetPro.Core)的环境使用

直接引用NetPro.Globalization nuget包，加入如下json配置即可

appsetting.json

```json
{
	"Globalization": {
		"UIQueryStringKey": "language",//请求的query携带的多语言参数名
		"ConnectionString": "Data Source=LocalizationRecords.sqlite", //sqlite地址
		"Cultures": [
			"zh-CN",
			"en-US"
		],
		"Annotations": true, //是否打开注册数据注解本地化服务
		"Record": false //不存在是否记录(自动插入数据库默认语系)，默认true
	}
}
```

### 业务使用

```csharp
    [ApiController]
    [Route("/api/v1/[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly IStringLocalizer<Localization.SqlLocalizer.IntegrationTests.SharedResource> _aboutLocalizerizer;//共享多语言资源

        public WeatherForecastController(IStringLocalizer<Localization.SqlLocalizer.IntegrationTests.SharedResource> aboutLocalizerizer)
        {
            _aboutLocalizerizer = aboutLocalizerizer;
        }

        [HttpGet("pay/create")]
        public string Get()
        {
            var cultureui = CultureInfo.CurrentUICulture.ToString();
            var culture = CultureInfo.CurrentCulture.ToString();
            //原生用法，底层会处理sqlite持久化
            return _aboutLocalizerizer["Name"];//会从SharedResource资源下查询Name对应的多语言，查询不到进入指定sqlite中查询，继续查询不到插入Name.当前语言代码
        }
    }
```

### 客户端处理

客户端请求接口时，依次支持，query，header，cookie等三种方式携带多语言标识
query 默认 ui-culture; 支持修改`UIQueryStringKey`节点覆盖默认参数名
header 默认 Accept-Language
cookie 默认为 ".AspNetCore.Culture" 既：（Microsoft.AspNetCore.Localization.CookieRequestCultureProvider.DefaultCookieName）


### tips

> [在线sqlite工具](https://sqliteonline.com/) https://sqliteonline.com/

> 感谢[KamenRiderKuuga](https://github.com/KamenRiderKuuga)开发的多语言翻译辅助工具[Translator](https://github.com/KamenRiderKuuga/Translator)