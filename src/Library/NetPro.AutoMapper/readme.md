
## NetPro.AutoMapper使用
 [![NuGet](https://img.shields.io/nuget/v/NetPro.AutoMapper.svg)](https://nuget.org/packages/NetPro.AutoMapper)

 引用此nuget包即可自动实现各种注册配置，引用后直接按下方步骤使用即可，无需再关心初始化等操作
  备注：默认增强启动模式，故需要入口处添加以下代码，如果以脚手架创建的项目可忽略此设置
```
Environment.SetEnvironmentVariable("ASPNETCORE_HOSTINGSTARTUPASSEMBLIES", "NetPro.Startup");
```

 - 1、配置映射关系
  继承IOrderedMapperProfile以实现批量注册
```c#
/// <summary>
/// 实体之间隐射配置
/// 数据库实体与AO业务实体互相映射
/// </summary>
public class UserMapper : Profile, IOrderedMapperProfile
{
    /// <summary>
    /// 
    /// </summary>
    public UserMapper()
    {
        //数据库实体映射AO业务实体,ReverseMap可实现双向映射
        CreateMap<UserInsertAo, User>().ReverseMap();
        CreateMap<UserUpdateAo, User>().ReverseMap();
    }

    /// <summary>
    /// 映射顺序，默认0即可，无需更改
    /// </summary>
    public int Order => 0;
}
```
- 2、使用automapper映射转换

```
public class FreeSQLDemoService : IFreeSQLDemoService
    {
        private readonly IMapper _mapper;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="fsql"></param>
        /// <param name="idleFsql"></param>
        /// <param name="mapper"></param>
        public FreeSQLDemoService(IMapper mapper)
        {
            _mapper = mapper;
        }

        public void InsertAsync(UserInsertAo userInsertAo)
        {
            //AO实体隐射为数据库DO实体
            var userEntity = _mapper.Map<UserInsertAo, User>(userInsertAo);
        }
 }

```