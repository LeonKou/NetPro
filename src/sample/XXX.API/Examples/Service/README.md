# 说明
## Service文件夹说明：
存放业务服务逻辑
建议：
都以Service严格结尾

以一个实现配套一个接口，例如
``` csharp
public interface IDemoService{

}

public class DemoService：IDemoService{
    
}
```
