
# NetPro.TypeFinder使用

 [![NuGet](https://img.shields.io/nuget/v/NetPro.TypeFinder.svg)](https://nuget.org/packages/NetPro.TypeFinder)
目录、文件、程序集、类型的处理

## 使用

### 启用服务

```csharp
  public void ConfigureServices(IServiceCollection services, IConfiguration configuration = null){
   services.AddFileProcessService(new TypeFinderOption{MountePath="./Plugin"});
  }
            
```
建议从appsetting取配置，如下
```json
	"TypeFinderOption": {
		"MountePath": "./Plugin" //windows默认目录： C:/opt/netpro ; linux环境：/opt/netpro
        "CustomDllPattern":"^XXX.*"//自定义的程序dll正则表达式,此处表示XXX开头的程序集
	}
```

### 构造函数注入使用
```csharp
 public class DemoController : ControllerBase{
   private readonly INetProFileProvider _netProFileProvider;
   private readonly ITypeFinder _typeFinder;
   public DemoController(INetProFileProvider netProFileProvider,ITypeFinder typeFinder)
        {
           _netProFileProvider=_netProFileProvider;
           _typeFinder=typeFinder;
        }

        public void Method1(){
          _netProFileProvider.CreateFile("");//创建文件
          typeFinder.GetAssemblies();//获取所有程序集
        }
 }
```
