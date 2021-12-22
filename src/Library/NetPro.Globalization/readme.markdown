
# NetPro.Globalization使用
 [![NuGet](https://img.shields.io/nuget/v/NetPro.Globalization.svg)](https://nuget.org/packages/NetPro.Globalization)

国际化多语言使用，支持官方资源文件的方式外底层增加了sqlite持久化存储方式，便于跨语言跨项目共享和检查遗漏的多语言

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
    application.UseRouting();
    //置于app.UseRouting()后便可;
        var configuration = application.ApplicationServices.GetService<IConfiguration>();

        var globalization = configuration.GetSection(nameof(Globalization)).Get<Globalization>();

        var cultures = globalization?.Cultures ?? new string[] { };

        var localizationOptions = new RequestLocalizationOptions()
            .AddSupportedUICultures(cultures)
            .AddSupportedCultures(cultures);

        localizationOptions.RequestCultureProviders.Insert(0, new QueryStringRequestCultureProvider());
        localizationOptions.RequestCultureProviders.Insert(1, new CookieRequestCultureProvider());
        localizationOptions.RequestCultureProviders.Insert(2, new AcceptLanguageHeaderRequestCultureProvider());
        application.UseRequestLocalization(localizationOptions);
   }
```

appsetting.json

```json
{
	"Globalization": {
		"ConnectionString": "Data Source=LocalizationRecords.sqlite",	//存储多语言的sqlite地址,初始化会默认生成数据库
		"Cultures": [
			"zh-CN",
			"en-US"
		]
	}
}

```

#### 依赖于[![NuGet](https://img.shields.io/nuget/v/NetPro.Core.svg)](https://nuget.org/packages/NetPro.Core)的环境使用

直接引用NetPro.Globalization nuget包，加入如下json配置即可

appsetting.json

```json
{
	"Globalization": {
		"ConnectionString": "Data Source=LocalizationRecords.sqlite",	//存储多语言的sqlite地址,初始化会默认生成数据库
		"Cultures": [
			"zh-CN",
			"en-US"
		]
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

### tips

[在线sqlite工具](https://sqliteonline.com/) https://sqliteonline.com/