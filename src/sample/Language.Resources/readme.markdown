
## Globalization(全球化)使用示例

### 介绍

.Netcore自带全球化支持。
.Netcore支持的多语言支持query,header,cookie三种方式获取目标语言
，ASP.NETCore会从URL中的culture参数中获取当前应用使用的语言文化，

除了指定ui-culture参数，你还可以使用culture参数指定当前格式化时间，数字等所使用的语言文化.以下以zh-cn为例

#### query：
当url中包含`ui-culture=zh-cn` 
#### cookie：
`c=zh-CN|uic=zh-CN`,其中c表示culture, uic表示ui-culture
#### header：
`Accept-Language:zh-cn`

---
 Tips: 当只指定culture或ui-culture参数时，ASP.NET Core会自动将culture和ui-culture设置成一样的。即?culture=zh-CN等同于?culture=zh-CN&ui-culture=zh-CN

 

### 使用
---
#### 中间件注入
```csharp
 public void ConfigureServices(IServiceCollection services, IConfiguration configuration = null)
        {
            services.AddLocalization(options => options.ResourcesPath = "");//资源文件路径，不填默认共享
        }
```
``` csharp
var supportedCultures = new[] { "en-us", "zh-CN" };
            var localizationOptions = new RequestLocalizationOptions()
                .SetDefaultCulture("zh-CN")//默认支持语言
                .AddSupportedUICultures(supportedCultures)//可通过浏览器传入culture=en-US 获得目标语言
                ;
            //放于Routing，endpoint中间件之前
            application.UseRequestLocalization(localizationOptions);
```
---
#### 语言资源文件准备
- 新建一个类，例如Language.Resouces
- 新建一个资源文件Language.zh-CN.resx(zh-CN部分和中间件初始化注入的支持语言要一样)
- 将资源文件设置为`嵌入的资源`,访问修饰符设置为`public`

---
#### 构造函数注入

```csharp
public class XXController : ControllerBase
{
        private readonly IStringLocalizer<Language.Resoureces.Language> _localizer;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="redisManager"></param>
        public XXController(
           IStringLocalizer<Language.Resoureces.Language> localizer)
        {
            _localizer = localizer;
        }

        /// <summary>
        /// 多语言测试
        /// </summary>
        /// <param name="gg"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("test")]
        public IActionResult Get([FromQuery] XXXRequest gg)
        {;
            //此处会根据客户端传过来的目标语言找到资源文件对应的语言资源显示
            return Ok(_localizer["who are you"]+$"{DateTime.Now}");
        }
}

```

