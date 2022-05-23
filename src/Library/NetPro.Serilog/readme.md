
## NetPro.Dependency使用
 [![NuGet](https://img.shields.io/nuget/v/NetPro.Dependency.svg)](https://nuget.org/packages/NetPro.Dependency)

依赖注入辅助程序集，依赖 [![NuGet](https://img.shields.io/nuget/v/NetPro.TypeFinder.svg)](https://nuget.org/packages/NetPro.TypeFinder)

##### 依赖注入 

除了使用官方原生依赖注入外，支持通过接口和正则模式注入

*接口方式注入*

1、`ITransientDependency`

2、`ISingletonDependency`

3、`IScopedDependency`


```C#

  public interface IFreeSQLDemoByDependency {}
  public class FreeSQLDemoByDependency : IFreeSQLDemoByDependency, IScopedDependency//通过继承注入接口实现依赖注入
 {

 }
```
*正则批量注入*
```C#
services.BatchInjection("^XXX.", "Service$"); //批量注入以XXX前缀的程序集，Service结尾的类
```
